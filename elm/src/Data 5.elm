module Data exposing (conflictList)

import Types exposing (..)


conflictList : List Conflict
conflictList = [
    { description = "More than 150 financial institutions hold debts connected to the President-elect, according to a Wall Street Journal analysis. Trump's business interests hold debt upwards of $650 million, owed in part to Bank of China and Goldman Sachs.", familyMember = "Donald Sr.", conflictingEntity = "(various)", category = "Active", notes = "", dateAddedOrEdited = "7/5/2017",  sources = [{ sourceName = "Wall Street Journal",  date = "1/5/2017" }
    ,{ sourceName = "New York Times",  date = "11/6/2016" } 
    ,{ sourceName = "New York Times",  date = "8/20/2016" }] }  
    ]
