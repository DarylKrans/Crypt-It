# Crypt-It
Highly improvised encryption program. 

I started learning c# a year ago and I made a few programs that involved file manipulation.  I tried my hand at an encryption program.
While it worked OK, it's not great and the encryption method is rather weak.

Crypt-It is my 2nd attempt at encryption and I feel that this version has a much stronger scramble method than the last.  It also introduces
new methods of encrypting like bit rotation and seed based keys.  I don't feel I am on the level of AES encryption, I am still in the 
learning process.  

This program is fully functional however it is possible you may encounter a few bugs (none of which have resulted in lost or corrupt data)
I would strongly recommend making copies of files before testing just to be sure.

As with my last encryption program, keys and passwords are not stored anywhere.  Keys are generated based on your password.  If you forget
your password, you will not be able to recover your encrypted data unless you are a wizzard or you can find the seed your password generated!
Out of 2,147,483,647 possible seeds, it could take a while!  I'm also working on a program that will try to brute force seeds to see how long
it takes to do because I am curious

So, remember your password!  If you decrypt a file with the wrong password, it will let you, but that will only result in a more scrambled
file output.

This program is just for fun, so enjoy at your own risk.  Also, if you need to keep something safe from prying eyes, this program will
certainly do the trick!
