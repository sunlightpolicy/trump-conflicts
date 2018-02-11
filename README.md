# trump-conflicts

This is a search tool based on the Sunlight Foundation's [Trump's Conflict of Interest](https://sunlightfoundation.com/tracking-trumps-conflicts-of-interest/) project. The [data](https://docs.google.com/spreadsheets/d/1-_vJDLlCtd94zaieFeB2qdLB9WUdNPIryWBFNuXAAZ8/edit#gid=0) has been converted into native Elm types found in src/Types.elm. Sunlight will update the data periodically, and is working on adding location information, which should make the data set more useful.

To Do

- Freeze the search criteria at the top, and scroll the results
- Default to selecting the first item in the search result when old selection is no longer in the list
- Better color, layout etc!
- Filter by search date
- Highlight matched text (yellow like chrome?)
- Change data to CSV, and use a decoder
- Show current filters
- Use Style Elements