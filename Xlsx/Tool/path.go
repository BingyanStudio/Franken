package main

import (
	"os"
	"strings"
)

const (
	root = "../../" // 根目录相对路径

	old = "Xlsx/Source/"
	new = "Project/Assets/Config/CSV/"

	src = root + old // xlsx源文件目录
	dst = root + new // CSV目标文件目录

	xlsxSuf = ".xlsx"
	csvSuf  = ".csv"
)

func isXlsx(path string) bool {
	return strings.HasSuffix(path, xlsxSuf)
}

func replacePath(path string) string {
	return strings.ReplaceAll(path, old, new)
}

func toCSV(path string) string {
	path = replacePath(path)
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
