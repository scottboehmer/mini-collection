# mini-collection
A simple dotnet command line tool for tracking a collection of miniatures using json files.

## MiniCollectionTool Settings

Add a `settings.json` file to the MiniCollectionTool directory in
order to provide a directory for your collection files.

```json
{
    "Path": INSERT_YOUR_PATH
}
```

## MiniCollectionTool Commands

### Add a Miniature

```
dotnet run -- add-mini --mini "Awesome AWS-8Q"
```

### Add a Force

```
dotnet run -- add-force --force "Ryuken-roku" --faction "Kurita"
```

### Assign a Miniature to a Force

```
dotnet run -- add-mini-to-force --mini "Awesome AWS-8Q" --force "Ryuken-roku"
```

### Mark a Miniature as Painted

```
dotnet run -- paint-mini --mini "Awesome AWS-8Q" --force "Ryuken-roku"
```

### Generate Markdown Files

```
dotnet run -- render
```

