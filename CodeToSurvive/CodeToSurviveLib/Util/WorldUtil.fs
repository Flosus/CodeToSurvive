namespace CodeToSurviveLib.Util

open CodeToSurviveLib.Core.Domain
open CodeToSurviveLib.Core.Plugin
open Microsoft.Extensions.Logging

module WorldUtil =

    exception NoSpawnChunkGeneratorFound

    let mutable private spawnChunk: ChunkId option = None

    let private tryFindFirstOption predicate (array: _[]) =
        let rec loop i =
            if i >= array.Length then
                None
            else
                let res = predicate array[i]

                match res with
                | None -> loop (i + 1)
                | Some _ -> res

        loop 0

    let private tryAddChunk ctx (chunk: Chunk) =
        let existingChunk =
            ctx.State.Map.Chunks
            |> Seq.tryFind (fun existingCnk -> existingCnk.Id = chunk.Id)

        match existingChunk with
        | Some _ -> ()
        | None -> ctx.State.Map.Chunks.Add chunk


    let getChunk (chunkId: ChunkId) (ctx: WorldContext) (char: CharacterState) =
        let log = ctx.CreateLogger "WorldUtil"

        let chunkInMapOpt =
            ctx.State.Map.Chunks |> Seq.tryFind (fun (cnk: Chunk) -> cnk.Id = chunkId)

        match chunkInMapOpt with
        | Some _ -> chunkInMapOpt
        | None ->
            let generatedChunk =
                PluginRegistry.getPlugins ()
                |> Array.filter (fun plugin -> plugin.GenerateChunk.IsSome)
                |> Array.map (fun plugin -> plugin.GenerateChunk.Value)
                |> Array.rev
                |> tryFindFirstOption (fun genChunk -> genChunk ctx char chunkId)

            match generatedChunk with
            | None ->
                log.LogWarning $"No chunk generator found for '{chunkId}'"
                None
            | Some chunk ->
                log.LogDebug $"Chunk generated for '{chunkId}'"
                tryAddChunk ctx chunk
                generatedChunk

    let private getOrGenerateSpawnChunk (ctx: WorldContext) =
        let log = ctx.CreateLogger "WorldUtil"

        let generatedSpawnChunkOpt =
            PluginRegistry.getPlugins ()
            |> Array.filter (fun plugin -> plugin.GetSpawnChunk.IsSome)
            |> Array.map (fun plugin -> plugin.GetSpawnChunk.Value)
            |> Array.rev
            |> tryFindFirstOption (fun genChunk -> genChunk ctx)

        match generatedSpawnChunkOpt with
        | None ->
            log.LogError "No spawn chunk found!"
            None
        | Some chunk ->
            log.LogDebug $"Found spawnChunk with id '{chunk.Id}'"
            tryAddChunk ctx chunk
            spawnChunk <- Some(chunk.Id)
            generatedSpawnChunkOpt

    let getSpawnChunk (ctx: WorldContext) =

        let spawnChunkOpt =
            match spawnChunk with
            | Some spawnChunkId ->
                let foundSpawnChunk =
                    ctx.State.Map.Chunks |> Seq.tryFind (fun (cnk: Chunk) -> cnk.Id = spawnChunkId)

                match foundSpawnChunk with
                | Some _ -> foundSpawnChunk
                | None -> getOrGenerateSpawnChunk ctx
            | None -> getOrGenerateSpawnChunk ctx

        match spawnChunkOpt with
        | Some spawnChunk -> spawnChunk
        | None -> raise NoSpawnChunkGeneratorFound



    let getChunkOrDefault (chunkId: ChunkId) (ctx: WorldContext) (char: CharacterState) =
        let log = ctx.CreateLogger "WorldUtil"
        let chunkOpt = getChunk chunkId ctx char

        match chunkOpt with
        | Some foundChunk -> foundChunk
        | None ->
            log.LogWarning $"Chunk '{chunkId}' not found, falling back to spawn chunk"
            getSpawnChunk ctx
