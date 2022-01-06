                    ▓▓▓▓
                  ▓▓▓▓▓▓
                ▓▓▓▓▒▒▓▓▓▓
              ▓▓▓▓▒▒░░▓▓▓▓
            ▓▓▓▓▒▒░░░░▓▓▓▓
          ▓▓▓▓▒▒▓▓▓▓▒▒▓▓▓▓
        ▓▓▓▓▒▒░░▓▓▓▓▒▒▒▒▓▓▓▓
      ▓▓▓▓▓▓░░▒▒▓▓▓▓░░▒▒▓▓▓▓
    ▓▓▓▓▓▓░░░░▒▒▓▓▓▓░░░░▓▓▓▓
  ▓▓▓▓▓▓░░░░░░░░░░░░░░░░▒▒▓▓▓▓
▓▓▓▓▓▓░░░░░░░░▓▓▓▓░░░░░░▒▒▓▓▓▓
▓▓▓▓▓▓▒▒░░░░░░▓▓▓▓░░░░░░░░▓▓▓▓
▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒░░░░░░░░░░▓▓▓▓
      ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▒▒▒▒▒▒▓▓▓▓
              ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓
                        ▓▓▓▓▓▓▓▓

disaster engine version 5

c# raylib game engine by jazz "danger" mickle

building:
* install dotnet core
* navigate to the disaster5/ folder
* run `dotnet publish -c Release`
* copy over the `disaster_test` repo into a `data` subfolder in the output
* create `settings.cfg` with `basedir data` inside
* run that dang executable

running from source (vscode):
* install dotnet core
* open this repo in vscode
* add your games `basedir` to workspace (File -> Add Folder to Workspace...)
* press `Ctrl+Shift+B` and select run (basedir is set to the root folder of currently opened file)

running from source otherwise:
* install dotnet core
* navigate to the disaster5/ folder
* run `dotnet run -c {Release | Debug} -r {win-x64 | linux-x64} -- path/to/basedir`

contributors:
Luna 'Ryuko' Zaremba -- https://lunaryuko.space/
Yali 'Sciman' Izzo -- https://sciman.info/
msylvia -- https://github.com/MSylvia
Jarrod 'Jayrude' Doyle -- https://twitter.com/Jayrude_

certainly incomplete list of ppl who've helped make disaster engine become a reality:
ronja, margarat dax, zombie, marrub, anotak, cepheus, luna, jayde, radscale, sievaxx, sciman, jp lebreton
