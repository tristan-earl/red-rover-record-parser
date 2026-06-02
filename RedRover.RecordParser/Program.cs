using RedRover.RecordParser;

string flag = args[0];
if (flag == "-d")
{
    var record = Record.Parse(args[1]);
    Console.WriteLine(record.ToString());
    Console.WriteLine(record.ToAlternateOrderString());
}
else if (flag == "-f")
{
    string file = args[1];
    using var reader = new StreamReader(File.OpenRead(file));
    using var writer = new StreamWriter(File.OpenWrite(@".\record_parser_results.txt"));
    string? line = null;
    while ((line = reader.ReadLine()) != null)
    {
        Record record;
        try
        {
            record = Record.Parse(line);
        }
        catch (RecordParserException ex)
        {
            Console.WriteLine($"Failed to parse record: {line}\nerror={ex.Message}");
            continue;
        }

        writer.WriteLine(record.ToString());
        writer.WriteLine(record.ToAlternateOrderString());
    }
}