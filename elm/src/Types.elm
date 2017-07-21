module Types exposing (..)


type alias Source =
    { sourceName : String
    , date : String
    }


type alias Conflict =
    { description : String
    , familyMember : String
    , conflictingEntity : String
    , category : String
    , notes : String
    , dateAddedOrEdited : String
    , sources : List Source
    }

