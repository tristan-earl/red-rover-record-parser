using RedRover.RecordParser;

string flag = args[0];
if (flag == "-d")
{
    string line = args[1];
    Record record;
    try
    {
        record = Record.Parse(line);
    }
    catch (RecordParserException ex)
    {
        Console.WriteLine($"Failed to parse record: {line}\nerror={ex.Message}");
        return;
    }

    Console.WriteLine(record.ToString());
    Console.WriteLine(record.ToAlternateOrderString());
}
else if (flag == "-f")
{
    string file = args[1];
    using var reader = new StreamReader(File.OpenRead(file));
    using var writer = new StreamWriter(File.OpenWrite(@".\record_parser_results.txt"));
    string? line = null;
    int lineNumber = 0;
    int numErrors = 0;
    while ((line = reader.ReadLine()) != null)
    {
        lineNumber++;

        Record record;
        try
        {
            record = Record.Parse(line);
        }
        catch (RecordParserException ex)
        {
            Console.WriteLine($"Failed to parse record at line {lineNumber}: {line}\nerror={ex.Message}\n");
            numErrors++;
            continue;
        }

        writer.WriteLine(record.ToString());
        writer.WriteLine(record.ToAlternateOrderString());
    }

    Console.WriteLine($"Parsed {lineNumber} records with {numErrors} errors");
}