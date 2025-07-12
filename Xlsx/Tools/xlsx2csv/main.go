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

	return currentDirInfo{currentDirs, currentFiles}, nil
}

func processXlsx(path string) error {
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
	for i, row := range rows[3:] { // 忽略前三行：名称、类型、标识符
		for j, colCell := range row {
			fmt.Fprint(file, colCell)
			if j != len(row)-1 { // 避免多余分隔符
				fmt.Fprint(file, "\t")
			}
		}
		if i != len(rows)-4 { // 避免多余空行
			fmt.Fprintln(file)
		}
	}

	file.Sync()

	return nil
}

func main() {
	fmt.Println("开始转换xlsx！")

	var info currentDirInfo
	info.currentDirs = append(info.currentDirs, root)

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
		if isXlsx(path) {
			processXlsx(path)
		}
	}

	fmt.Println("已转换为csv！")
}
