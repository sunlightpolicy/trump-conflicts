# trump-conflicts

This is a search tool written in Elm and based on the Sunlight Foundation's [Trump's Conflict of Interest](https://sunlightfoundation.com/tracking-trumps-conflicts-of-interest/) project. The [data](https://docs.google.com/spreadsheets/d/1-_vJDLlCtd94zaieFeB2qdLB9WUdNPIryWBFNuXAAZ8/edit#gid=0) has been converted into native Elm types found in src/Types.elm. Sunlight will update the data periodically, and is working on adding location information, which should make the data set more useful.

To Do

1. Add description on the right above the list of sources
2. Search all the fields
3. Freeze the seach criteria at the top, and scroll the results
4. Default to selecting the first item in the search result on the right whenever anything changes
5. Better color, layout etc!
6. Show the number of results
7. Filter by search date
8. Highlight matched text (yellow like chrome?)
9. Search should not be case sensitive!
10. Improve the Elm..
