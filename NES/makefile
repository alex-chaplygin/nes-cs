src=Memory.cs Program.cs CPU.cs Cartridge.cs

all: /tmp/Program.exe

/tmp/Program.exe:	$(src) Program.cs
	dmcs $^ -out:$@ -r:System.Drawing.dll
