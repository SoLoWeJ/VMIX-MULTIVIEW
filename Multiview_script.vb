'=== USER SETTINGS / НАСТРОЙКИ ПОЛЬЗОВАТЕЛЯ ===
Dim defaultTextColor   as String = "#FFFFFFFF"      ' Normal text color / Обычный цвет текста
Dim defaultBgColor     as String = "#80808080"      ' Normal background / Обычный фон
Dim programTextColor   as String = "#FFFFFFFF"      ' Program text / Текст программы
Dim programBgColor     as String = "#CC0000"        ' Program background / Фон программы
Dim previewTextColor   as String = "#000000"        ' Preview text / Текст превью
Dim previewBgColor     as String = "#FFFFFFFF"      ' Preview background / Фон превью
Dim refresh            as Integer = 50              ' Update speed (ms) / Скорость обновления (мс)

'=== RUNTIME VARIABLES / ПЕРЕМЕННЫЕ ===
Dim mvConfigName        as String = "MV_CONFIG"     ' Config input name / Имя конфигурации
Dim blankInputName      as String = "BLACK"         ' Blank input name / Имя черного входа
Dim mvInputs()          as String = {}              ' Multiview list / Список мультивью
Dim totalMVConfigLayers as Integer = 0              ' Layers in config / Слоев в конфиге
Dim initialized         as Boolean = false          ' First run done / Первый запуск выполнен
Dim lastRedetection     as Integer = 0              ' Last check time / Время последней проверки
Dim redetectionInterval as Integer = 30000          ' Recheck interval / Интервал перепроверки
Dim lastMVCount         as Integer = 0              ' Previous MV count / Предыдущее количество MV
Dim lastLayerCount      as Integer = 0              ' Previous layers / Предыдущее количество слоев

'=== STARTUP MESSAGE / СООБЩЕНИЕ ПРИ ЗАПУСКЕ ===
Console.WriteLine("MV CONFIGURATOR STARTED")
Console.WriteLine("СКРИПТ НАСТРОЙКИ МУЛЬТИВЬЮ ЗАПУЩЕН")

'=== MAIN LOOP / ГЛАВНЫЙ ЦИКЛ ===
While True
    ' Initial delay on first run / Задержка при первом запуске
    If Not initialized Then
        Sleep(5000)
    End If

    ' Periodic re-detection / Периодическая перепроверка
    Dim currentTime As Integer = Environment.TickCount
    If initialized AndAlso (currentTime - lastRedetection) > redetectionInterval Then
        initialized = false
        lastRedetection = currentTime
    End If

    ' Load vMix XML configuration / Загрузка XML конфигурации vMix
    Dim config As System.Xml.XmlDocument = New System.Xml.XmlDocument
    config.loadxml(API.XML)

    ' Get preview and program numbers / Получение номеров превью и программы
    Dim previewNumber As String = ""
    Dim programNumber As String = ""
    
    Dim previewNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/preview")
    Dim activeNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/active")
    
    If previewNode IsNot Nothing Then previewNumber = previewNode.InnerText
    If activeNode IsNot Nothing Then programNumber = activeNode.InnerText
    
    If previewNumber = "" Or programNumber = "" Then
        Sleep(refresh)
        Continue While
    End If
    
    ' Get input names from numbers / Получение имен входов по номерам
    Dim previewName As String = ""
    Dim programName As String = ""
    
    Dim previewInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@number='" + previewNumber + "']")
    Dim programInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@number='" + programNumber + "']")
    
    If previewInputNode IsNot Nothing AndAlso previewInputNode.Attributes.GetNamedItem("shortTitle") IsNot Nothing Then
        previewName = previewInputNode.Attributes.GetNamedItem("shortTitle").Value
    End If
    
    If programInputNode IsNot Nothing AndAlso programInputNode.Attributes.GetNamedItem("shortTitle") IsNot Nothing Then
        programName = programInputNode.Attributes.GetNamedItem("shortTitle").Value
    End If
    
    ' Update MV2_CONFIG display / Обновление дисплея MV2_CONFIG
    Try
        Dim presetNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/preset")
        Dim projectName As String = ""
        If presetNode IsNot Nothing Then
            projectName = System.IO.Path.GetFileNameWithoutExtension(presetNode.InnerText)
        End If
        
        Input.Find("MV2_CONFIG").Text("txt_prv.Text") = previewName
        Input.Find("MV2_CONFIG").Text("txt_pgm.Text") = programName
        Input.Find("MV2_CONFIG").Text("txt_proj.Text") = projectName
        Input.Find("MV_CONFIG").Text("txt_pgm_name.Text") = projectName
    Catch
    End Try

    ' Find MV_CONFIG input / Поиск входа MV_CONFIG
    Dim titleInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + mvConfigName + "']")
    
    If titleInputNode Is Nothing Then
        Sleep(refresh)
        Continue While
    End If
    
    Dim titleInputKey As String = ""
    If titleInputNode.Attributes.GetNamedItem("key") IsNot Nothing Then
        titleInputKey = titleInputNode.Attributes.GetNamedItem("key").Value
    End If
    
    If titleInputKey = "" Then
        Sleep(refresh)
        Continue While
    End If

    '=== DETECTION PHASE / ФАЗА ОБНАРУЖЕНИЯ ===
    If Not initialized Then
        ' Find all MV inputs (excluding MV0 and MV_CONFIG) / Поиск всех MV входов (кроме MV0 и MV_CONFIG)
        Dim mvList As New System.Collections.Generic.List(Of String)
        Dim allInputsNode As System.Xml.XmlNodeList = config.SelectNodes("/vmix/inputs/input[contains(@shortTitle, 'MV')]")
        
        If allInputsNode IsNot Nothing Then
            For Each inputNode As System.Xml.XmlNode In allInputsNode
                If inputNode.Attributes.GetNamedItem("shortTitle") IsNot Nothing Then
                    Dim shortTitle As String = inputNode.Attributes.GetNamedItem("shortTitle").Value
                    If shortTitle.StartsWith("MV") AndAlso shortTitle <> mvConfigName AndAlso shortTitle <> "MV0" Then
                        If shortTitle.Length > 2 Then
                            Dim numberPart As String = shortTitle.Substring(2)
                            Dim testNumber As Integer
                            If Integer.TryParse(numberPart, testNumber) Then
                                mvList.Add(shortTitle)
                            End If
                        End If
                    End If
                End If
            Next
        End If
        
        ' Sort MVs numerically / Сортировка MV по номерам
        If mvList.Count > 0 Then
            For c As Integer = 0 To mvList.Count - 2
                For j As Integer = c + 1 To mvList.Count - 1
                    Dim numI As Integer = Integer.Parse(mvList(c).Substring(2))
                    Dim numJ As Integer = Integer.Parse(mvList(j).Substring(2))
                    If numI > numJ Then
                        Dim tempMV As String = mvList(c)
                        mvList(c) = mvList(j)
                        mvList(j) = tempMV
                    End If
                Next
            Next
        End If
        
        mvInputs = mvList.ToArray()
        
        ' Count layers in MV_CONFIG / Подсчет слоев в MV_CONFIG
        totalMVConfigLayers = 0
        For i As Integer = 1 To 64
            Dim textField As String = "txt_IN" & i & ".Text"
            Dim textNode As System.Xml.XmlNode = titleInputNode.SelectSingleNode("text[@name='" + textField + "']")
            If textNode IsNot Nothing Then
                totalMVConfigLayers = i
            Else
                Exit For
            End If
        Next
        
        ' Show detection results / Показать результаты обнаружения
        If mvInputs.Length <> lastMVCount Or totalMVConfigLayers <> lastLayerCount Then
            Console.WriteLine("Found / Найдено: " + mvInputs.Length.ToString() + " MVs, " + totalMVConfigLayers.ToString() + " layers / слоев")
            lastMVCount = mvInputs.Length
            lastLayerCount = totalMVConfigLayers
        End If
        
        initialized = true
    End If

    '=== COLOR CODING / ЦВЕТОВАЯ РАЗМЕТКА ===
    For i As Integer = 1 To totalMVConfigLayers
        Dim textField As String = "txt_IN" & i & ".Text"
        Dim colorRectangle As String = "rct_IN" & i & ".Fill.Color"
        
        Dim textNode As System.Xml.XmlNode = titleInputNode.SelectSingleNode("text[@name='" + textField + "']")
        If textNode Is Nothing Then Exit For
        
        Dim inputText As String = textNode.InnerText
        
        ' Set colors based on program/preview status / Установка цветов в зависимости от статуса
        If inputText = programName Then
            API.Function("SetTextColour", Input:=titleInputKey, Value:=programTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=programBgColor, SelectedName:=colorRectangle)
        ElseIf inputText = previewName Then
            API.Function("SetTextColour", Input:=titleInputKey, Value:=previewTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=previewBgColor, SelectedName:=colorRectangle)
        Else
            API.Function("SetTextColour", Input:=titleInputKey, Value:=defaultTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=defaultBgColor, SelectedName:=colorRectangle)
        End If
    Next

    '=== MULTIVIEW ASSIGNMENT / НАЗНАЧЕНИЕ МУЛЬТИВЬЮ ===
    If mvInputs.Length > 0 AndAlso totalMVConfigLayers > 0 Then
        ' Calculate layer distribution / Расчет распределения слоев
        Dim layersPerMV As Integer = totalMVConfigLayers \ mvInputs.Length
        Dim remainderLayers As Integer = totalMVConfigLayers Mod mvInputs.Length
        
        Dim currentLayerIndex As Integer = 1
        
        ' Assign layers to each MV / Назначение слоев каждому MV
        For mvIndex As Integer = 0 To mvInputs.Length - 1
            Dim mvName As String = mvInputs(mvIndex)
            Dim mvInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + mvName + "']")
            
            If mvInputNode IsNot Nothing AndAlso mvInputNode.Attributes.GetNamedItem("key") IsNot Nothing Then
                Dim mvKey As String = mvInputNode.Attributes.GetNamedItem("key").Value
                
                ' Calculate layers for this MV / Расчет слоев для этого MV
                Dim layersForThisMV As Integer = layersPerMV
                If mvIndex < remainderLayers Then
                    layersForThisMV += 1
                End If
                
                ' Assign inputs / Назначение входов
                For layerOffset As Integer = 0 To layersForThisMV - 1
                    Dim configLayerIndex As Integer = currentLayerIndex + layerOffset
                    If configLayerIndex > totalMVConfigLayers Then Exit For
                    
                    Dim mvTextField As String = "txt_IN" & configLayerIndex & ".Text"
                    Dim inputTextNode As System.Xml.XmlNode = titleInputNode.SelectSingleNode("text[@name='" + mvTextField + "']")
                    
                    If inputTextNode IsNot Nothing Then
                        Dim inputName As String = inputTextNode.InnerText.Trim()
                        Dim inputKey As String = ""
                        
                        ' Find input key / Поиск ключа входа
                        If inputName <> "" Then
                            Dim targetInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + inputName + "']")
                            If targetInputNode IsNot Nothing AndAlso targetInputNode.Attributes.GetNamedItem("key") IsNot Nothing Then
                                inputKey = targetInputNode.Attributes.GetNamedItem("key").Value
                            End If
                        End If
                        
                        ' Fallback to black input / Резервный черный вход
                        If inputKey = "" Then
                            Dim blackInputNode As System.Xml.XmlNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + blankInputName + "']")
                            If blackInputNode IsNot Nothing AndAlso blackInputNode.Attributes.GetNamedItem("key") IsNot Nothing Then
                                inputKey = blackInputNode.Attributes.GetNamedItem("key").Value
                            End If
                        End If
                        
                        ' Assign to multiview layer / Назначение на слой мультивью
                        If inputKey <> "" Then
                            API.Function("SetMultiViewOverlay", Input:=mvKey, Value:=(layerOffset + 1).ToString() + "," + inputKey)
                        End If
                    End If
                Next
                
                currentLayerIndex += layersForThisMV
            End If
        Next
    End If
    
    Sleep(refresh)
End While
