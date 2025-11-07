'ver. 1.02a - VERSION / Версия 1.02a
Console.WriteLine("ver. 1.02a TALLY CONFIG")                         
'================================= Multiview Configurator Script / Скрипт конфигурации мультивью ==========================================
'================================================ USER DEFINED SETTINGS(VARIABLES) ========================================================
Dim defaultTextColor                      as String = "#FFFFFFFF"                         ' Default text color / Цвет текста по умолчанию
Dim defaultBgColor                        as String = "#80808080"                         ' Default background color / Цвет фона по умолчанию
Dim programTextColor                      as String = "#FFFFFFFF"                         ' Program text color / Цвет текста программы
Dim programBgColor                        as String = "#CC0000"                           ' Program background color / Цвет фона программы
Dim previewTextColor                      as String = "#FFFFFFFF"                         ' Preview text color / Цвет текста превью
Dim previewBgColor                        as String = "#6AA84F"                           ' Preview background color / Цвет фона превью
Dim refresh                               as integer = 100                                  ' Refresh interval / Интервал обновления

Dim mvConfigName                          as String = "MV_CONFIG"                           ' GT title input name / Имя GT заголовка
Dim mvInputs()                            as String = {"MV1", "MV2","MV3"}                  ' Array of multiview inputs / Массив multiview входов
Dim blankInputName                        as String = "BLACK"                               ' Blank/black input name / Имя пустого/черного входа
Dim layerCount                            as integer = 4                                    ' Total layers per multiview / Всего слоев на multiview
Dim inputNumberInMVConfigStart            as integer = 1                                    ' Start of txt_IN layers / Начало слоев txt_IN
Dim inputNumberInMVConfigend              as integer = 12                                   ' End of txt_IN layers / Конец слоев txt_IN
'========================================== DEFAULT VARIABLES  ============================================================================
Dim colorRactangle                        as String                                         ' Rectangle color element / Элемент цвета прямоугольника
Dim textField                             as String                                         ' Text field element / Элемент текстового поля
Dim inputNumberInMVConfig                 as integer                                        ' txt_IN layer counter / Счетчик слоев txt_IN
Dim titleInputKey                         as String                                         ' Key of the title input / Ключ входа заголовка
'========================================== PROGRAM VARIABLES =============================================================================
Dim programNumber                         as String                                         ' Program input number / Номер входа программы
Dim programName                           as String                                         ' Program input name / Имя входа программы
Dim programKey                            as String                                         ' Program input key / Ключ входа программы
'========================================== PREVIEW VARIABLES =============================================================================
Dim previewNumber                         as String                                         ' Preview input number / Номер входа превью
Dim previewName                           as String                                         ' Preview input name / Имя входа превью
Dim previewKey                            as String                                         ' Preview input key / Ключ входа превью
'======================================= INPUT OBJECT VARIABLES ===========================================================================
Dim inputText                             as String                                         ' Text value storage / Хранение текстового значения
'======================================= MW MULTIVIEW VARIABLES ===========================================================================
Dim mvName                                as String                                         ' Current multiview name / Текущее имя multiview
Dim mvKey                                 as String                                         ' Multiview input key / Ключ multiview входа
Dim mvIndex                               as integer                                        ' Multiview index counter / Счетчик индекса multiview
Dim layer                                 as integer                                        ' Layer number / Номер слоя
Dim gtFieldIndex                          as integer                                        ' GT field index calculation / Расчет индекса поля GT
Dim mvTextField                           as String                                         ' Multiview text field name / Имя текстового поля multiview
Dim inputName                             as String                                         ' Input name from GT / Имя входа из GT
Dim inputKey                              as String                                         ' Input key for SetLayer function / Ключ входа для функции SetLayer
Dim isValidInput                          as Boolean                                        ' Input validation flag / Флаг проверки входа
Dim upperInputName                        as String                                         ' Uppercase input name for comparison / Имя входа в верхнем регистре для сравнения
Dim errorHandler                          as Boolean = false                                ' Для отслеживания предыдущего состояния ошибки
dim currentErrorState                     as boolean

Console.WriteLine("====================================================================")
Console.WriteLine("MV CONFIGURATOR SCRIPT STARTED - Version 1.02a")
Console.WriteLine("====================================================================")
Console.WriteLine("Configuration loaded:")
Console.WriteLine("MV Config: " + mvConfigName)
Console.WriteLine("Multiviews: " + String.Join(", ", mvInputs))
Console.WriteLine("Blank Input: " + blankInputName)
Console.WriteLine("Refresh Rate: " + refresh.ToString() + "ms")
Console.WriteLine("====================================================================")

'=========================================== MAIN LOOP START ==============================================================================
' Infinite loop / Бесконечный цикл
While True
    Dim config                             as New System.Xml.XmlDocument                    ' XML document for vMix configuration / XML документ для конфигурации vMix
    Dim titleInputNode                     as System.Xml.XmlNode                            ' Node for title input / Узел для входа заголовка
    Dim previewInputNode                   as System.Xml.XmlNode                            ' Node for preview input / Узел для входа превью
    Dim programInputNode                   as System.Xml.XmlNode                            ' Node for program input / Узел для входа программы
    Dim textNode                           as System.Xml.XmlNode                            ' Node for text elements / Узел для текстовых элементов
    Dim mvInputNode                        as System.Xml.XmlNode                            ' Node for multiview input / Узел для multiview входа
    Dim inputTextNode                      as System.Xml.XmlNode                            ' Node for input text / Узел для текста входа
    Dim targetInputNode                    as System.Xml.XmlNode                            ' Node for target input / Узел для целевого входа
'===========================================================================================================================================

    ' Load current vMix configuration / Загружаем текущую конфигурацию vMix
    config.loadxml(API.XML)

    ' Get preview and program numbers / Получаем номера превью и программы
    previewNumber = config.SelectSingleNode("/vmix/preview").InnerText
    programNumber = config.SelectSingleNode("/vmix/active").InnerText

    ' Get preview and program input nodes / Получаем узлы входов превью и программы
    previewInputNode = config.SelectSingleNode("/vmix/inputs/input[@number='" + previewNumber + "']")
    programInputNode = config.SelectSingleNode("/vmix/inputs/input[@number='" + programNumber + "']")

    ' Get short titles and keys / Получаем короткие названия и ключи
    If previewInputNode IsNot Nothing Then
        previewName = previewInputNode.Attributes.GetNamedItem("shortTitle").Value
        previewKey = previewInputNode.Attributes.GetNamedItem("key").Value
    Else
        previewName = ""
        previewKey = ""
    End If
    
    If programInputNode IsNot Nothing Then
        programName = programInputNode.Attributes.GetNamedItem("shortTitle").Value
        programKey = programInputNode.Attributes.GetNamedItem("key").Value
    Else
        programName = ""
        programKey = ""
    End If

'=========================================================================================================================================

    ' Find the GT title input / Находим вход GT title
    titleInputNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + mvConfigName + "']")

'============================ STOP SCRIPT IF MV_CONFIG NOT FOUND =========================================================================
'======================== ОСТАНОВИТЬ СКРИПТ, ЕСЛИ MV_CONFIG НЕ НАЙДЕН ====================================================================
    currentErrorState = (titleInputNode is Nothing)
      If currentErrorState <> errorHandler Then
       If currentErrorState

        Console.WriteLine("============================================================")
        Console.WriteLine("CRITICAL ERROR: " + mvConfigName + " NOT FOUND!")
        Console.WriteLine("SCRIPT STOPPING - MV_CONFIG input is required for operation")
        Console.WriteLine("КРИТИЧЕСКАЯ ОШИБКА: " + mvConfigName + " НЕ НАЙДЕН!")
        Console.WriteLine("СКРИПТ ОСТАНАВЛИВАЕТСЯ - Для работы требуется вход MV_CONFIG")
        Console.WriteLine("============================================================")

       Else

        Console.WriteLine("============================================================")
        Console.WriteLine("CRITICAL ERROR FIXED: " + mvConfigName + " FOUND!")
        Console.WriteLine("ИСПРАВЛЕНА КРИТИЧЕСКАЯ ОШИБКА: " + mvConfigName + " НАЙДЕНО!")
        Console.WriteLine("============================================================")
       End If
       errorHandler = currentErrorState
       End If

     If currentErrorState then
       Sleep(refresh)
     Continue While
     End If

'========================================================================================================================================= 
    ' Get the key of the title input / Получаем ключ входа заголовка
    titleInputKey = titleInputNode.Attributes.GetNamedItem("key").Value

' ====================== COLOR CODING LOGIC ==============================================================================================
' ================= ЛОГИКА ЦВЕТОВОГО КОДИРОВАНИЯ =========================================================================================

' Loop through all text fields in the MV_CONFIG / Перебираем все текстовые поля в MV_CONFIG
For inputNumberInMVConfig = inputNumberInMVConfigStart To inputNumberInMVConfigend
    ' Construct text field and color rectangle names / Формируем имена текстового поля и цветного прямоугольника
    textField = "txt_IN" & inputNumberInMVConfig & ".Text"
    colorRactangle = "rct_IN" & inputNumberInMVConfig & ".Fill.Color"
    
    ' CHECK: Does the text field exist in GT title? / ПРОВЕРКА: Существует ли текстовое поле в GT title?
    textNode = titleInputNode.SelectSingleNode("text[@name='" + textField + "']")
    If textNode IsNot Nothing Then
        inputText = textNode.InnerText
        
        ' Apply color coding based on input status / Применяем цветовое кодирование в зависимости от статуса входа
        If inputText = programName Then
            ' Program input - red background / Вход программы - красный фон
            API.Function("SetTextColour", Input:=titleInputKey, Value:=programTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=programBgColor, SelectedName:=colorRactangle)
        ElseIf inputText = previewName Then
            ' Preview input - green background / Вход превью - зеленый фон
            API.Function("SetTextColour", Input:=titleInputKey, Value:=previewTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=previewBgColor, SelectedName:=colorRactangle)
        Else
            ' Default input - gray background / Обычный вход - серый фон
            API.Function("SetTextColour", Input:=titleInputKey, Value:=defaultTextColor, SelectedName:=textField)
            API.Function("SetColor", Input:=titleInputKey, Value:=defaultBgColor, SelectedName:=colorRactangle)
        End If
    Else
        ' Field doesn't exist - skip / Поле не существует - пропускаем
        Console.WriteLine("Field not found: " + textField)
        Console.WriteLine("Поле не найдено: " + textField)
    End If
'========================================================================================================================================
Next

'======================= MV INPUT ASSIGNMENT LOGIC ======================================================================================
'================= ЛОГИКА НАЗНАЧЕНИЯ ВХОДОВ МУЛЬТИВЬЮ ===================================================================================
    mvIndex = 0
    
    ' Process each multiview input / Обрабатываем каждый multiview вход
    For Each mvName In mvInputs
        mvInputNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + mvName + "']")
        
        If mvInputNode IsNot Nothing Then
            ' Get multiview input key / Получаем ключ multiview входа
            mvKey = mvInputNode.Attributes.GetNamedItem("key").Value

            ' Process each layer in the multiview / Обрабатываем каждый слой в мультивью
            For layer = 1 To layerCount
                ' Calculate GT field index / Вычисляем индекс поля GT
                gtFieldIndex = (mvIndex * layerCount) + layer
                
                ' Check if index is within valid range / Проверяем, находится ли индекс в допустимом диапазоне
                If gtFieldIndex <= inputNumberInMVConfigend Then
                    mvTextField = "txt_IN" & gtFieldIndex & ".Text"

                    ' Get input name from text field / Получаем имя входа из текстового поля
                    inputTextNode = titleInputNode.SelectSingleNode("text[@name='" + mvTextField + "']")
                    If inputTextNode IsNot Nothing Then
                        inputName = inputTextNode.InnerText
                    Else
                        inputName = ""
                    End If
'======================================================================================================================================
'=================== INPUT VALIDATION AND FALLBACK ====================================================================================
'================= ПРОВЕРКА ВХОДА И РЕЗЕРВНЫЙ ВАРИАНТ =================================================================================

                    isValidInput = False
                    inputKey = ""
                    
                    ' Check if input exists and is valid / Проверяем, существует ли вход и корректен ли он
                    If Not String.IsNullOrEmpty(inputName) AndAlso inputName.Trim() <> "" Then
                        ' Convert to uppercase for case-insensitive comparison / Преобразуем в верхний регистр для сравнения без учета регистра
                        upperInputName = inputName.ToUpper().Trim()
                        targetInputNode = config.SelectSingleNode("/vmix/inputs/input[translate(@shortTitle, 'abcdefghijklmnopqrstuvwxyz', 'ABCDEFGHIJKLMNOPQRSTUVWXYZ')='" + upperInputName + "']")
                        
                        If targetInputNode IsNot Nothing Then
                            isValidInput = True
                            inputKey = targetInputNode.Attributes.GetNamedItem("key").Value
                        End If
                    End If
                    
                    ' Fallback to black input if invalid / Резервный вариант - черный вход, если невалидный
                    If Not isValidInput Then
                        inputName = blankInputName
                        targetInputNode = config.SelectSingleNode("/vmix/inputs/input[@shortTitle='" + blankInputName + "']")
                        If targetInputNode IsNot Nothing Then
                            inputKey = targetInputNode.Attributes.GetNamedItem("key").Value
                        Else
                            ' Skip this layer if black input not found / Пропускаем этот слой, если черный вход не найден
                            Continue For
                        End If
                    End If
                    
                    ' Validation before API call / Проверка перед вызовом API
                    If Not String.IsNullOrEmpty(mvKey) AndAlso Not String.IsNullOrEmpty(inputKey) Then

                        ' Assign input to multiview layer / Назначаем вход на слой мультивью
                        API.Function("SetMultiViewOverlay", Input:=mvKey, Value:=layer.ToString() + "," + inputKey)
                    Else
                        Console.WriteLine("WARNING: Skipping layer " + layer.ToString() + " - invalid parameters")
                        Console.WriteLine("ПРЕДУПРЕЖДЕНИЕ: Пропускаем слой " + layer.ToString() + " - неверные параметры")
                    End If
                End If
            Next

            ' Increment multiview index / Увеличиваем индекс мультивью
            mvIndex += 1
        End If
    Next
    
    ' Wait before next iteration / Ожидание перед следующей итерацией
    Sleep(refresh)
End While
