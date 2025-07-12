package main

import (
	"os"
	"strings"
)

const (
	tpl = "csv.tpl"

	src     = "../../Source/"                // xlsx源文件目录
	dst     = "../../../Project/Script/CSV/" // cs文件路径
	csvPath = dst + "CSV.g.cs"

	res = "res://Assets/Config/CSV/"

	xlsxSuf = ".xlsx"
	csvSuf  = ".txt"
	csSuf   = ".cs"
)

func isXlsx(path string) bool {
	return strings.HasSuffix(path, xlsxSuf)
}

func toCSV(path string) string {
	path, _ = strings.CutSuffix(path, xlsxSuf)
	return path + csvSuf
}

func exists(path string) (bool, error) {
	_, err := os.Stat(path)
	if err != nil {
		if os.IsExist(err) {
			return true, err
		}
		if os.IsNotExist(err) {
			return false, nil
		}
		return false, err
	}
	return true, nil
}

func replaceSlash(path string) string {
	return strings.ReplaceAll(path, "\\", "/")
}

func getName(path string) string {
	path, _ = strings.CutSuffix(path, xlsxSuf)
	return path
}
