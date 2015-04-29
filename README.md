# Serial-PIC16F84-Calc
### Version
0.1

Project based on [Microsoft's MSDN Example](https://code.msdn.microsoft.com/windowsdesktop/SerialPort-brief-Example-ac0d5004) and [Microsoft's SerialPort Class Example](https://msdn.microsoft.com/en-us/library/system.io.ports.serialport%28v=vs.110%29.aspx).

Serial-PIC16F84-Calc is a simple calculator implemented in C#. For this project we use a (Matrix Keypad)[http://i.imgur.com/JS5WchN.png].

The basic code for this project opens up a window where the user can select the COM port to be used, baud rate, data bits, parity and stop bits. It contains also an empty window where the data output will be shown.

Upon receiving data from the keypad, the output window writes down the number received. It then receives an operand (Keys 'A', 'B', 'C' and 'D') and performs given operation upon receiving the '=' sign (Key '#').

Upon receiving next number the output is cleared. Output may be cleared by pressing the "Limpiar" (clean) button. To begin the operation of the program press "Iniciar" (begin) to begin listening the selected port with the given configuration. To stop press "Parar" (stop).

Information
----
Contact:
   - [Alan Andrés Mar Herrrera](alan75_mar@hotmail.com)
   - [Jesus Angel Salazar Marcatoma](missing@gmail.com)

License: [MIT](https://github.com/dagonar/Serial-PIC16F84-Calc/blob/master/LICENSE.md)
