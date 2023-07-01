Available Inputs:

- FLAC file
- mp3 file
- FLAC files
- mp3 files
- folder
- URL
- musicbrainz search

* if Release GUID is different across files in input, throw an error.
* if Release GUID is not found acriss file/s in input, throw an error.
* when Release GUID is found, output to console
* when Release GUID is found, fill URL Textbox field
------------------------
Screen:
- Input Area
- URL Textbox
-- Artist
-- Album
-- Label
-- Cat. No.
-- Country (artist or release?)
-- Year
-- Type
-- Styles (multiselect)
- Output Area
- Album Description Output Area
- Generate Output button
- Tags In The Text Count Label
------------------------
Process:
Cycle I
1. On Input File
2. Lookup Musicbrainz ID
3. Download Musicbrainz Release
4. Fill preliminary fields
Cycle II
1. Generate Button pressed
2. Generate Description
3. Alert if more than 10 tags are in the Description
4. Generate Subalbum Description