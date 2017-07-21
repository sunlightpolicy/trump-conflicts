module Main exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import List exposing (..)

import Types exposing (..)
import Data exposing (..)


type alias Model =
    { conflictList : List Conflict
      ,selectedConflict : Maybe Conflict    
    }


initModel : Model
initModel = 
    { conflictList = conflictList
      ,selectedConflict = Nothing
    }


-- update

type Msg
    = Search String
    | SelectConflict Conflict


update : Msg -> Model -> Model
update msg model =
  case msg of
    Search search ->
        model
    SelectConflict conflict->
         { model | selectedConflict = Just conflict} 


-- view


view : Model -> Html Msg
view model =
    table
        [ style[ ( "width", "100%" ) ] ] 
        [ tr [] 
            [ td [] (drawConflictRows model.conflictList)
            , td [] (drawSources (model.selectedConflict))
            ]
        ]
    

drawConflictRows : List Conflict -> List (Html Msg)
drawConflictRows conflicts =
    conflicts
        |> List.map drawConflictRow


drawConflictRow : Conflict -> Html Msg
drawConflictRow conflict = 
    tr [ onClick (SelectConflict conflict)] 
        [ td [style[ ( "width", "70px" ) ]] [text (conflict.familyMember) ] 
        , td [style[ ( "width", "500px" ) ]] [text (conflict.conflictingEntity) ] 
        , td [style[ ( "width", "70px" ) ]] [text (conflict.category) ]
        , td [style[ ( "width", "70px" ) ]] [text (conflict.dateAddedOrEdited) ]
        ] 


drawSources : Maybe Conflict -> List (Html Msg)
drawSources conflict =
    case conflict of
        Nothing ->
            [ h3 [] [text <| "Nothing"] ]

        Just conflict ->
            conflict.sources
                |> List.map drawSourceRow


drawSourceRow : Source -> Html Msg
drawSourceRow source = 
    tr [] 
        [ td [style[ ( "width", "70px" ) ]] [text (source.sourceName) ] 
        , td [style[ ( "width", "500px" ) ]] [text (source.date) ] 
        ] 


main : Program Never Model Msg
main =
    Html.beginnerProgram
        { model = initModel
        , view = view
        , update = update
        }

