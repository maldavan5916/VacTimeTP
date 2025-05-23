import re

# Функция для удаления многострочных комментариев, исключая строки
def remove_multiline_comments(code):
    in_string = False
    in_comment = False
    result = []
    i = 0
    while i < len(code):
        if not in_comment and not in_string and code[i:i+2] == '/*':
            in_comment = True
            i += 2
        elif in_comment and code[i:i+2] == '*/':
            in_comment = False
            i += 2
        elif not in_comment and code[i] == '"':
            in_string = not in_string
            result.append(code[i])
            i += 1
        elif not in_comment:
            result.append(code[i])
            i += 1
        else:
            i += 1
    return ''.join(result)

# Функция для удаления однострочных комментариев, исключая строки
def remove_single_line_comments(code):
    in_string = False
    result = []
    i = 0
    while i < len(code):
        if not in_string and code[i:i+2] == '//':
            while i < len(code) and code[i] != '\n':
                i += 1
        elif code[i] == '"':
            in_string = not in_string
            result.append(code[i])
            i += 1
        else:
            result.append(code[i])
            i += 1
    return ''.join(result)

# Функция для сжатия кода, удаляя лишние пробелы вокруг операторов
def compress_code(code):
    operators = ['=', ';', '{', '}', '(', ')', ',', '.', '+', '-', '*', '/', '>', '<', '!', '&', '|']
    in_string = False
    result = []
    i = 0
    while i < len(code):
        if code[i] == '"':
            in_string = not in_string
            result.append(code[i])
            i += 1
        elif in_string:
            result.append(code[i])
            i += 1
        elif code[i] in operators:
            while result and result[-1] == ' ':
                result.pop()
            result.append(code[i])
            i += 1
            while i < len(code) and code[i] == ' ':
                i += 1
        else:
            result.append(code[i])
            i += 1
    return ''.join(result)

# Основной код
with open("SourceCode.txt", "r", encoding="utf-8") as f:
    code = f.read()

# Сбор using-директив
using_directives = set()
using_pattern = re.compile(r'^\s*using\s+[\w.]+\s*;', re.MULTILINE)
for match in using_pattern.finditer(code):
    using_directives.add(match.group().strip())

# Удаление using-директив из кода
code = re.sub(using_pattern, '', code)

# Удаление комментариев
code = remove_multiline_comments(code)
code = remove_single_line_comments(code)

# Замена переносов строк пробелами
code = re.sub(r'\n+', ' ', code)

# Сжатие кода
code = compress_code(code)

# Уборка лишних пробелов
code = re.sub(r'\s+', ' ', code).strip()

# Сборка итогового кода
final_code = ''.join(sorted(using_directives)) + code

# Запись в файл
with open("SourceCodeCompress.txt", "w", encoding="utf-8") as f:
    f.write(final_code)