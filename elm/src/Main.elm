module Main exposing (..)

import Html exposing (..)
import Html.Attributes exposing (..)
import Html.Events exposing (..)
import List exposing (..)

import Types exposing (..)
import Data exposing (..)


type alias Model =
    { conflictList : List Conflict
    , selectedList : List Conflict
    , selectedConflict : Maybe Conflict    
    , familyMember : FamilyMember
    }

type FamilyMember
    = Sr 
    | Jr
    | Ivanka
    | Jared
    | Melania
    | Eric
 

initModel : Model
initModel = 
    { conflictList = conflictList
    , selectedList = conflictList
    , selectedConflict = Nothing
    , familyMember = Sr
    }


-- update

type Msg
    = Search String
    | SelectConflict Conflict
    | ChooseFamilyMember FamilyMember


update : Msg -> Model -> Model
update msg model =
  case msg of
    Search search ->
        if search == "" then
            { model | selectedList = conflictList }
        else
            --filteredConflicts model search model.familyMember
            { model | selectedList = List.filter (\record -> String.contains (String.toUpper search) (String.toUpper (record.conflictingEntity ++ record.description))) model.selectedList }
    
    SelectConflict conflict ->
        { model | selectedConflict = Just conflict} 

    ChooseFamilyMember selectedFamilyMember ->
        { model | familyMember = selectedFamilyMember }             


--filteredConflicts : Model -> String -> FamilyMember -> Model
--filteredConflicts model search person =
--   let 
--        filterBySearchString : List Conflict -> String search -> List Conflict
--        filterBySearchString conflicts search = 
--            { model | selectedList = List.filter (\record -> String.contains (String.toUpper search) (String.toUpper (record.conflictingEntity ++ record.description))) model.selectedList }


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
                [ td [] [ input [type_ "text", placeholder "Search", onInput Search] []
                , td [] (List.map familyMemberChooser [ Sr, Jr, Ivanka, Jared, Melania, Eric ]) 
                ]
            ]
            ,tr [] 
                [ td [ conflictPaneStyle ] (drawConflictRows model.selectedList model.selectedConflict)
                , td [ sourcePaneStyle ] (drawSources (model.selectedConflict))
            ]
        ]
    

drawConflictRows : List Conflict -> Maybe Conflict -> List (Html Msg)
drawConflictRows conflicts selectedConflict =
    let 
        drawConflictRow : Conflict -> Html Msg
        drawConflictRow conflict = 
            tr [ onClick (SelectConflict conflict)] 
            [ td [style[ ( "width", "70px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.familyMember) ] 
            , td [style[ ( "width", "70px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.category) ]
            , td [style[ ( "width", "100px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.conflictingEntity) ] 
            , td [style[ ( "width", "400px" ) ], classList [ ("selected", isSelected selectedConflict conflict ) ] ] [text (conflict.description) ]
            ]
    in
        conflicts
            |> List.map drawConflictRow


isSelected : Maybe Conflict -> Conflict -> Bool
isSelected selectedConflict conflict = 
    case selectedConflict of
        Nothing ->
            False 

        Just selected ->
            selected == conflict     



drawSources : Maybe Conflict -> List (Html Msg)
drawSources conflict =
    let
        drawSourceRow : Source -> Html Msg
        drawSourceRow source = 
            tr [] 
            [ td [style[ ( "width", "150px" ) ]] [ a [ href source.link ] [text source.name ] ] 
            , td [style[ ( "width", "80px" ) ]] [text (source.date) ] 
            ]
    in
        case conflict of
            Nothing ->
                [ h3 [] [text <| ""] ]

            Just conflict ->
                conflict.sources
                    |> List.map drawSourceRow


familyMemberChooser : FamilyMember -> Html msg
familyMemberChooser person =
    label [] 
        [ input [ type_ "radio", name "familyMember" ] []
        , text (familyMemberToString person)
        ]


familyMemberToString : FamilyMember -> String
familyMemberToString person =
    case person of
        Sr -> 
            "Trump" 
        Jr ->
            "Junior"
        Ivanka ->
            "Ivanka"
        Jared ->
            "Jared"
        Melania ->
            "Melania"
        Eric ->
            "Eric"


main : Program Never Model Msg
main =
    Html.beginnerProgram
        { model = initModel
        , view = view
        , update = update
        }

