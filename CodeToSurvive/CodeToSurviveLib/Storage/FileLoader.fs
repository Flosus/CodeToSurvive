namespace CodeToSurvive.Lib.Storage



module FileLoader =

    type Entry =
        | Property of string * string
        | Line of string
        | GroupStart of string
        | GroupEnd
        | Comment

    type Group = { Entries: Entry[] }
    type FileContent = { Groups: Group[] }

    let private loadLine (lines: string[]) : FileContent =

        let rec _loadLine (lines: string[]) (currentState: FileContent) =
            match lines.Length with
            | 0 -> currentState
            | _ ->
                let line = lines[0]

                let entry =
                    match line.Trim() with
                    | "" -> Comment
                    | _ when line.StartsWith("#") -> Comment
                    | _ when line.StartsWith(">") ->
                        let groupName = line.Substring(1).Trim()
                        GroupStart groupName
                    | _ when line.StartsWith("<") -> GroupEnd
                    | _ when line.Contains("=") ->
                        let parts = line.Split("=")
                        Property(parts[0], parts[1])
                    | _ -> Line line

                let currentGroup =
                    match entry with
                    | Comment -> ()
                    | _ -> ()

                _loadLine lines[1..] currentState

        _loadLine lines { Groups = [||] }

    let loadFile (filePath: string) : FileContent =
        let fileContent = [| "" |]

        loadLine fileContent


    ()
