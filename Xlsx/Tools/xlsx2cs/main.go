package main

import (
	"fmt"
	"os"
	"path/filepath"
	"strings"
	"text/template"

	"github.com/xuri/excelize/v2"
)

type Field struct {
	Type       string
	Identifier string
}

type Xlsx struct {
	Name   string
	Path   string // Godot加载csv的路径
	Fields []Field
}
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

func processXlsx(path string) Xlsx {
	fmt.Printf("开始处理表格%s\n", path)

	f, err := excelize.OpenFile(path)
	if err != nil {
		panic(err)
	}

	abs, _ := filepath.Abs(src)
	rel, err := filepath.Rel(abs, path)
	if err != nil {
		panic(err)
	}

	var fields []Field

	cols, err := f.GetCols(f.WorkBook.Sheets.Sheet[0].Name)
	for _, row := range cols {
		fields = append(fields, Field{row[1], row[2]})
	}

	return Xlsx{getName(filepath.Base(path)), res + toCSV(replaceSlash(rel)), fields}
}

func main() {
	csvTpl, err := template.New("").Funcs(template.FuncMap{"HasPrefix": strings.HasPrefix}).ParseFiles(tpl)
	if err != nil {
		fmt.Println(err)
		return
	}

	fmt.Println("开始生成代码！")

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

	var xlsxs []Xlsx

	// 处理所有表格
	for _, path := range info.currentFiles {
		if isXlsx(path) {
			xlsxs = append(xlsxs, processXlsx(path))
		}
	}

	file, _ := os.OpenFile(csvPath, os.O_RDWR|os.O_TRUNC|os.O_CREATE, 0664)
	defer file.Close()

	fmt.Println("开始生成" + csvPath)
	err = csvTpl.ExecuteTemplate(file, tpl, xlsxs)
	if err != nil {
		fmt.Println(err)
		return
	}

	file.Sync()

	fmt.Println("代码生成完毕！")
}
