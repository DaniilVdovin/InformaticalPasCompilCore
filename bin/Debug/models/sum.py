import sys
if(sys.argv[1]=="-r"):
    print("r:0:100:2")
else:
    print(sum(map(int,sys.argv[1:])))
