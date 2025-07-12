package main

import (
	"fmt"
	"os"
	"path/filepath"

	"github.com/xuri/excelize/v2"
)

type currentDirInfo struct {
	currentDirs  []string
	currentFiles []string
}

func getCurrentDir(path string) (currentDirInfo, error) {
	path, _ = filepath.Abs(path)
	fmt.Printf("开始处理文件夹%s\n", path)
	path += "\\"

	infos, err := os.ReadDir(path)
	if err != nil {
		fmt.Println(err)
		return currentDirInfo{}, nil
	}

	var currentDirs []string
	var currentFiles []string

	for _, info := range infos {
		abs, err := filepath.Abs(path + info.Name())
		if err != nil {
			return currentDirInfo{}, nil
		}
		if info.IsDir() {
			currentDirs = append(currentDirs, abs)
		} else if isXlsx(abs) {
			currentFiles = append(currentFiles, abs)
		}
	}

	fmt.Printf("当前文件夹：\n")
	for _, v := range currentDirs {
		fmt.Printf("%s\n", v)
	}
	fmt.Printf("当前表格：\n")
	for _, v := range currentFiles {
		fmt.Printf("%s\n", v)
	}

	return currentDirInfo{currentDirs: currentDirs, currentFiles: currentFiles}, nil
}

func processXlsx(path string) error {
	if !isXlsx(path) {
		return nil
	}

	fmt.Printf("开始处理表格%s\n", path)

	f, err := excelize.OpenFile(path)
	if err != nil {
		fmt.Println(err)
		return nil
	}

	// 创建csv文件夹
	path = toCSV(replaceSlash(path))
	dir := filepath.Dir(path)
	exist, err := exists(dir)
	if err != nil || (!exist && os.MkdirAll(dir, 0755) != nil) {
		fmt.Println(err)
		return nil
	}

	file, _ := os.OpenFile(path, os.O_RDWR|os.O_TRUNC|os.O_CREATE, 0664)
	defer file.Close()

	rows, err := f.GetRows(f.WorkBook.Sheets.Sheet[0].Name)
	for _, row := range rows {
		for _, colCell := range row {
			fmt.Fprint(file, colCell, "\t")
		}
		fmt.Fprintln(file)
	}

	file.Sync()

	return nil
}

func main() {
	fmt.Println("开始转换xlsx！")

	var info currentDirInfo
	info.currentDirs = append(info.currentDirs, src)

	// 得到所有表格
	for {
		infos, err := getCurrentDir(info.currentDirs[0])
		if err != nil {
			fmt.Println(err)
			return
		}

		info.currentDirs = append(info.currentDirs, infos.currentDirs...)[1:]
		info.currentFiles = append(info.currentFiles, infos.currentFiles...)

		if len(info.currentDirs) == 0 {
			break
		}
	}

	// 处理所有表格
	for _, path := range info.currentFiles {
		processXlsx(path)
	}

	fmt.Println("已转换为csv！")
}
