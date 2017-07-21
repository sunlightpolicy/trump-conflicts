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
        {model | conflictList = List.filter (\record -> String.contains search record.conflictingEntity) model.conflictList }
        --{ model | selectedConflict = Nothing} 
    SelectConflict conflict->
        { model | selectedConflict = Just conflict} 


-- view

conflictPaneStyle : Attribute msg
conflictPaneStyle =
    style
        [ ( "float", "left" )
        ]

sourcePaneStyle : Attribute msg
sourcePaneStyle =
    style
        [ ( "float", "left" )
        , ( "width", "380px" )
        ]


view : Model -> Html Msg
view model =
    table
        [ style[ ( "width", "100%" ) ] ]
        [
            tr []
                [td [style[ ( "font-size", "xx-large" ) ]] [text "Tracking Trump's Conflicts of Interest"]
            ]
            ,tr []
                [td [] [
                    input [type_ "text", placeholder "Search", onInput Search] []
                ]
            ]
            ,tr [] 
                [ td [ conflictPaneStyle ] (drawConflictRows model.conflictList)
                , td [ sourcePaneStyle ] (drawSources (model.selectedConflict))
            ]
        ]
    

drawConflictRows : List Conflict -> List (Html Msg)
drawConflictRows conflicts =
    let
        drawConflictRow : Conflict -> Html Msg
        drawConflictRow conflict = 
            tr [ onClick (SelectConflict conflict)] 
            [ td [style[ ( "width", "70px" ) ]] [text (conflict.familyMember) ] 
            , td [style[ ( "width", "70px" ) ]] [text (conflict.category) ]
            , td [style[ ( "width", "500px" ) ]] [text (conflict.conflictingEntity) ] 
            , td [style[ ( "width", "70px" ) ]] [text (conflict.dateAddedOrEdited) ]
        ]
    in
        conflicts
            |> List.map drawConflictRow


drawSources : Maybe Conflict -> List (Html Msg)
drawSources conflict =
    let
        drawSourceRow : Source -> Html Msg
        drawSourceRow source = 
            tr [] 
            [ td [style[ ( "width", "300px" ) ]] [text (source.sourceName) ] 
            , td [style[ ( "width", "80px" ) ]] [text (source.date) ] 
            ]
    in
        case conflict of
            Nothing ->
                [ h3 [] [text <| "Nothing"] ]

            Just conflict ->
                conflict.sources
                    |> List.map drawSourceRow


main : Program Never Model Msg
main =
    Html.beginnerProgram
        { model = initModel
        , view = view
        , update = update
        }

