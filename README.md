# Red Rover Record Parser

Command line tool solution for record parsing code puzzle.

## Requirements
- .NET 10 SDK

## Usage

Open command line and navigate to `<repo root>\RedRover.RecordParser`

### Option 1

Pass in single record: `dotnet run -- -d "<data record>"`

Example:

Input
```
dotnet run -- -d "(12345, John Doe, john.doe@gmail.com, type(12, Teacher, customFields(\"Bachelor's degree\", 5 years experience, Math expertise)), abcde)"
```

Output
```
- 12345
- John Doe
- john.doe@gmail.com
- type
  - 12
  - Teacher
  - customFields
    - "Bachelor's degree"
    - 5 years experience
    - Math expertise
- abcde

- john.doe@gmail.com
- abcde
- 12345
- John Doe
- type
  - customFields
    - "Bachelor's degree"
    - 5 years experience
    - Math expertise
  - 12
  - Teacher

```

### Option 2

Pass in path to records file: `dotnet run -- -f "<path to records file>"`

Example:

Input
```
dotnet run -- -f "C:\source\test_records.txt"
```

test_records.txt
```
(id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)
(id, name, email, type(id, name, customFields(c1, c2, c3, c4, c5)), externalId)
(id, name, email, type(id, name, customFields(c1)), externalId)
```

Output written to `<repo root>\record_parser_results.txt`
```
- id
- name
- email
- type
  - id
  - name
  - customFields
    - c1
    - c2
    - c3
- externalId

- email
- externalId
- id
- name
- type
  - customFields
    - c1
    - c2
    - c3
  - id
  - name

- id
- name
- email
- type
  - id
  - name
  - customFields
    - c1
    - c2
    - c3
    - c4
    - c5
- externalId

- email
- externalId
- id
- name
- type
  - customFields
    - c1
    - c2
    - c3
    - c4
    - c5
  - id
  - name

- id
- name
- email
- type
  - id
  - name
  - customFields
    - c1
- externalId

- email
- externalId
- id
- name
- type
  - customFields
    - c1
  - id
  - name
```

## Tests

1. Open command line and navigate to `<repo root>\RedRover.RecordParser`.
2. Run `dotnet test`.