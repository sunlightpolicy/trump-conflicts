# trump-conflicts

This is a search tool written in Elm and based on the Sunlight Foundation's [Trump's Conflict of Interest](https://sunlightfoundation.com/tracking-trumps-conflicts-of-interest/) project. The [data](https://docs.google.com/spreadsheets/d/1-_vJDLlCtd94zaieFeB2qdLB9WUdNPIryWBFNuXAAZ8/edit#gid=0) has been converted into native Elm types found in src/Types.elm. Sunlight will update the data periodically, and is working on adding location information, which should make the data set more useful.

To Do

1. Freeze the seach criteria at the top, and scroll the results
2. Default to selecting the first item in the search result on the right whenever anything changes
3. Better color, layout etc!
4. Show the number of results
5. Filter by search date
6. Highlight matched text (yellow like chrome?)
7. Change data to CSV, and use a decoder
8. Show current filters
9. Use Style Elements
10. Add clear button for search
11. Show "All" selected on entry
12. clear links when selected is not in list (make selected = nothing)
