namespace RedRover.RecordParser.UnitTests
{
    public class RecordTests
    {
        public static readonly IEnumerable<object[]> SuccessTestCases =
        [
            // Basic example
            [
                "(id, name, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // No custom fields
            [
                "(id, name, email, type(type_id, type_name, customFields()), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string>()),
                    "externalId")
            ],

            // Single custom field
            [
                "(id, name, email, type(type_id, type_name, customFields(c1)), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1" }),
                    "externalId")
            ],

            // No spaces after commas
            [
                "(id,name,email,type(type_id,type_name,customFields(c1,c2,c3)),externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // Extra spaces
            [
                "  ( id , name , email , type ( type_id , type_name ,  customFields( c1, c2 , c3 )  ),  externalId  )",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // Blank data fields
            [
                "(,,,type(,,customFields(,,)),)",
                new Record(
                    "",
                    "",
                    "",
                    new RecordType("", "", new List<string> { "", "", "" }),
                    "")
            ],

            // Whitespace data fields
            [
                "(  ,   ,   , type( ,   ,customFields(  ,   ,   )), )",
                new Record(
                    "",
                    "",
                    "",
                    new RecordType("", "", new List<string> { "", "", "" }),
                    "")
            ],

            // "Real" data
            [
                "(12345, John Doe, john.doe@gmail.com, type(12, Teacher, customFields(\"Bachelor's degree\", 5 years experience, Math expertise)), abcde)",
                new Record(
                    "12345",
                    "John Doe",
                    "john.doe@gmail.com",
                    new RecordType("12", "Teacher", new List<string> { "\"Bachelor's degree\"", "5 years experience", "Math expertise" }),
                    "abcde")
            ]
        ];

        [Theory]
        [MemberData(nameof(SuccessTestCases))]
        public void Record_Parse_Success(string inputData, Record expected)
        {
            // Act
            var actual = Record.Parse(inputData);

            // Assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Type.Id, actual.Type.Id);
            Assert.Equal(expected.Type.Name, actual.Type.Name);
            Assert.Equal(expected.Type.CustomFields, actual.Type.CustomFields);
            Assert.Equal(expected.ExternalId, actual.ExternalId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("()")]
        [InlineData("(")]
        [InlineData(")")]

        // Missing name
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]

        // Missing type object
        [InlineData("(id, name, email, externalId)")]

        // Missing type.customFields object
        [InlineData("(id, email, type(type_id, type_name), externalId)")]

        // Unexpected fields after type.customFields
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3), unexpected_field), externalId)")]

        // Unexpected fields after externalId
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId, unexpected_field)")]

        // Missing parentheses
        [InlineData("id, name, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId")]
        [InlineData("(id, email, type type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3), externalId)")]

        // Data cut short
        [InlineData("(id, email, type(type_id, type_nam")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2,")]
        public void Record_Parse_Failure(string inputData)
        {
            // Act & Assert
            Assert.Throws<RecordParserException>(() => Record.Parse(inputData));
        }
    }
}
