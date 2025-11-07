' ======================= USER DEFINED VARIABLES ==============================

    dim MultiviewInputNames()   as string = {"MULTIVIEW", "MULTIVIEW2"}
    dim InputNamesTitleLayer    as integer = 10
    dim isLoop                  as boolean = True
    dim refreshPeriod           as integer = 250

        dim isRefreshInputs         as boolean = True
        dim prvPGMlayerName         as string = "MV_PRV_PGM"
        dim meTypeLayer             as integer = 1
    ' ================== COLORS ====================
        dim PGMColor        as string = "#FF0000" ' RED
        dim PRVColor        as string = "#00FF00" ' GREEN
        dim NormalColor     as string = "#FFFFFF" ' WHITE
    ' ==============================================

' =============================================================================

' ============================== DO NOT TOUCH THIS VARIABLES ==================
    dim loopCondition as boolean = True
    dim config as new System.Xml.Xmldocument
' =============================================================================

' ============================= MAIN LOOP =====================================
    while loopCondition
        config.loadxml(API.XML)
        dim PROGInputName as string = config.SelectSingleNode("/vmix/inputs/input[@number=" & config.SelectSingleNode("/vmix/active").InnerText & "]").Attributes.GetNamedItem("shortTitle").Value
        dim PREVInputName as string = config.SelectSingleNode("/vmix/inputs/input[@number=" & config.SelectSingleNode("/vmix/preview").InnerText & "]").Attributes.GetNamedItem("shortTitle").Value
        
        for each MultiviewInput as string In MultiviewInputNames
            dim MultiviewInputNode as XMLNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle="""  & MultiviewInput & """]")
            dim MultiviewInputNodeList as XMLNodeList   ' layers in MULTIVIEW Input (MVx1, MVx2 etc...)
            dim MultiviewLayerNode as XMLNode           ' Node of each MVxN of MULTIVIEW Input
            dim MultiviewLayerNodeName as string        ' Name of 
            dim TitleKey as string                      ' Key of Title with sources
            dim MultiViewLayerNodeList as XMLNodeList   ' layers in  MVxN Input 

            if MultiviewInputNode isnot Nothing Then
               MultiviewInputNodeList = MultiviewInputNode.SelectNodes("overlay") 
            end if            
            if MultiviewInputNodeList isNot Nothing Then
                for MultiviewLayerIndex as integer = 0 to MultiviewInputNodeList.count - 1
                    dim MultiviewLayerNodeKey as string = MultiviewInputNodeList.Item(MultiviewLayerIndex).Attributes.GetNamedItem("key").Value
                    
                    MultiviewLayerNode = config.SelectSingleNode("/vmix/inputs/input[@key="""  & MultiviewLayerNodeKey & """]")
                    if MultiviewLayerNode isNot Nothing then 
                        MultiViewLayerNodeList = MultiviewLayerNode.SelectNodes("overlay") 
                        MultiviewLayerNodeName = MultiviewLayerNode.Attributes.GetNamedItem("shortTitle").Value
                    end if
                    if MultiViewLayerNodeList isNot Nothing Then
                        for LayerIndex as integer = 0 to MultiViewLayerNodeList.Count -1 
                            dim LayerNodeKey as string = MultiViewLayerNodeList.Item(LayerIndex).Attributes.GetNamedItem("key").Value
                            dim LayerNodeIndex as string = MultiViewLayerNodeList.Item(LayerIndex).Attributes.GetNamedItem("index").Value
                            if LayerNodeIndex = (InputNamesTitleLayer-1).ToString() 
                                TitleKey = LayerNodeKey
                                dim TitleNode as XMLNode = config.SelectSingleNode("/vmix/inputs/input[@key=""" & TitleKey & """]")
                                if TitleNode isNot Nothing then
                                    dim TitleNodeTextList as XMLNodeList = TitleNode.SelectNodes("text")
                                    for textIndex as integer = 0 to TitleNodeTextList.count-1
                                        dim textNode as XMLNode =  TitleNodeTextList.Item(textIndex)
                                        dim textField as string = textNode.Attributes.GetNamedItem("name").Value
                                        dim text as string = textNode.InnerText
                                        dim colorToPaint as string = NormalColor
                                        select text
                                            case PROGInputName
                                                colorToPaint = PGMColor
                                            case PREVInputName
                                                colorToPaint = PRVColor
                                        end select
                                        API.Function("SetTextColour", Input:=TitleKey, Value:=colorToPaint, SelectedName:=textField)
                                        ' ==================== REFRESH INPUTS ================================================
                                            if isRefreshInputs then 
                                                if MultiviewLayerNodeName <> prvPGMlayerName then
                                                    dim inputToSet as string = text
                                                    ' ============= to save ME as Colour and not VirtualSet we shoult set X layer with type ===========
                                                        Dim MERegexString As String = "^me\d+$"
                                                        Dim MERegex As New System.Text.RegularExpressions.Regex(MERegexString)
                                                        
                                                        if MERegex.IsMatch(inputToSet.toLower()) and (meTypeLayer <> 0) then
                                                            dim meNode as XMLNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle=""" & inputToSet & """]")
                                                            if meNode isNot nothing then
                                                                dim meNodeList as XMLNodeList = meNode.SelectNodes("overlay")
                                                                if meNodeList isnot Nothing then
                                                                    for i as integer = 0 to meNodeList.count - 1
                                                                        dim localOverlayNode as XMLNode =  meNodeList.Item(i)
                                                                        dim localInputNode as XMLNode = config.SelectSingleNode("/vmix/inputs/input[@key=""" & meNodeList.Item(i).Attributes.GetNamedItem("key").Value & """]") 
                                                                        if localInputNode isNot Nothing then
                                                                            if  localOverlayNode.Attributes.GetNamedItem("index").Value = (meTypeLayer -1).toString() then
                                                                                inputToSet = localInputNode.Attributes.GetNamedItem("shortTitle").Value
                                                                            end if
                                                                        end if
                                                                    next
                                                                    
                                                                end if
                                                            end if
                                                        end if

                                                    API.Function("SetMultiViewOverlay", Input:=MultiviewLayerNodeName, Value:=(textIndex+1).toString() & "," & inputToSet)
                                                end if
                                            end if   
                                        ' =========================================================================================
                                    next
                                end if
                                
                                exit for
                            end if
                        next
                    end if                   
                next
            end if
        next
        loopCondition = isLoop
        if isLoop Then 
            sleep(refreshPeriod)
        end if
    end while
' =============================================================================