# Home Management App

## Opis projektu
Home Management App to aplikacja umożliwiająca zarządzanie zadaniami domowymi oraz nagrodami. Użytkownicy mogą tworzyć i śledzić swoje zadania, zdobywać punkty za ich wykonanie i wymieniać punkty na nagrody. Administratorzy mają dodatkowe uprawnienia do zarządzania użytkownikami, zadaniami, nagrodami oraz ich odbiorem.

## Autor
- Jerzy Jędruch
- Paweł Kowalski

## Wersja
1.0.0

## Instrukcja instalacji i uruchomienia

1. **Otwórz Visual Studio 2022.**
2. **Wybierz "Klonuj repozytorium".**
3. **W lokalizacji repozytorium wklej:**
    ```bash
    https://github.com/lavi9191/HomeManagementApp.git
    ```
4. **Przejdź do Narzędzia -> Menedżer pakietów NuGet -> Konsola Menedżera Pakietów**
    
5. **Przywróć pakiety NuGet:**
    ```powershell
    dotnet restore
    ```
6. **Uruchom migracje bazy danych:**
    ```powershell
    Update-Database
    ```
7. **Uruchom aplikację klikając w `HomeManagementApp`.**
   
**Konta do testowania**
**Admini:**
**Login: admin@o2.pl**
**Hasło: zaq1@WSX**

**Login: tata@o2.pl**
**Hasło: zaq1@WSX**

**Użytkownicy:**
**Login: mama@o2.pl**
**Hasło: zaq1@WSX**

**Login: syn@o2.pl**
**Hasło: zaq1@WSX**

**Login: corka@o2.pl**
**Hasło: zaq1@WSX**

## Struktura projektu
Projekt składa się z kilku głównych modułów:
- **Użytkownicy**: Zarządzanie użytkownikami i ich rolami.
- **Zadania**: Tworzenie, edycja i śledzenie zadań domowych.
- **Nagrody**: System punktów i nagród, zarządzanie nagrodami przez administratorów.
- **Role i Uprawnienia**: Nadawanie i odbieranie ról oraz uprawnień użytkownikom.

## System użytkowników
Aplikacja wykorzystuje system użytkowników z rolami i uprawnieniami. Użytkownicy mogą logować się i widzieć tylko swoje zadania. Administratorzy mają dostęp do dodatkowych funkcji, takich jak zarządzanie zadaniami, nagrodami, rolami i punktami.

## Zarządzanie nagrodami
Aplikacja umożliwia użytkownikom wymianę punktów na nagrody. Administratorzy mogą zarządzać nagrodami oraz odebranymi nagrodami. 
### Dodawanie nagrody:
- Administrator może dodawać nowe nagrody poprzez panel administratora.
### Usuwanie nagrody:
- Administrator może usuwać nagrody z listy nagród.
### Zarządzanie odebranymi nagrodami:
- Administrator może przeglądać listę odebranych nagród przez użytkowników oraz usuwać odebrane nagrody, co przywraca punkty użytkownikowi.

## Zarządzanie uprawnieniami
Aplikacja umożliwia administratorom nadawanie i odbieranie ról użytkownikom.
### Nadawanie roli Admin:
- Administrator może przypisać rolę admina użytkownikowi na podstawie adresu e-mail.
### Odbieranie roli Admin:
- Administrator może odebrać rolę admina użytkownikowi na podstawie adresu e-mail.

## Zaawansowane zapytania ORM
Projekt wykorzystuje zaawansowane zapytania ORM do efektywnego zarządzania danymi w bazie danych. Dzięki temu możliwe jest szybkie i bezpieczne przetwarzanie informacji o użytkownikach, zadaniach i nagrodach.

## Przykłady użycia
Aplikacja może być używana w codziennym zarządzaniu obowiązkami domowymi, motywowaniu członków rodziny do wykonywania zadań poprzez system punktów i nagród oraz efektywnym zarządzaniu nagrodami i uprawnieniami użytkowników.

## Widoki
Aplikacja oferuje intuicyjne interfejsy użytkownika dla różnych ról:
- **Użytkownik**: Widok zadań, punktów i dostępnych nagród.
- **Administrator**: Panel do zarządzania zadaniami, nagrodami, użytkownikami oraz ich rolami.

## Baza danych
Aplikacja korzysta z relacyjnej bazy danych do przechowywania informacji o użytkownikach, zadaniach, punktach i nagrodach. Struktura bazy danych jest zoptymalizowana pod kątem wydajności i bezpieczeństwa.
