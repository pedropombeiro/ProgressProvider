.nuget\nuget update -Self
.nuget\nuget pack -Build -Symbols -Version 1.0.17.4 -Properties Configuration=Release
pause
