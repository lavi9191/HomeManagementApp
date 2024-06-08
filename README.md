# Home Management App

## Opis projektu
Aplikacja do zarządzania zadaniami domowymi, która pozwala użytkownikom na tworzenie, przypisywanie, śledzenie postępów i otrzymywanie przypomnień o zadaniach. Dodatkowo umożliwia zarządzanie nagrodami oraz uprawnieniami użytkowników.

## Autor
- Jerzy Jędruch
- Paweł Kowalski

## Wersja
1.0.0

## Instrukcja instalacji i uruchomienia
1. Skopiuj repozytorium na lokalny komputer:
    ```bash
    git clone https://github.com/lavi9191/HomeManagementApp.git
    ```
2. Przejdź do katalogu projektu:
    ```bash
    cd HomeManagementApp/HomeManagementApp
    ```
3. Przywróć pakiety NuGet:
    ```bash
    dotnet restore
    ```
4. Uruchom migracje bazy danych:
    ```bash
    dotnet ef database update
    ```
5. Uruchom aplikację:
    ```bash
    dotnet run
    ```

## Struktura projektu
Aplikacja jest zbudowana w oparciu o wzorzec MVC. Struktura projektu wygląda następująco:
- **Controllers**: Zawiera kontrolery odpowiedzialne za obsługę żądań HTTP.
- **Models**: Zawiera modele danych używane w aplikacji.
- **Views**: Zawiera widoki odpowiadające za prezentację danych użytkownikowi.
- **Migrations**: Zawiera migracje bazy danych.

## System użytkowników
Aplikacja wykorzystuje system użytkowników z rolami i uprawnieniami. Użytkownicy mogą logować się i widzieć tylko swoje zadania. Administratorzy mają dostęp do dodatkowych funkcji, takich jak zarządzanie zadaniami, nagrodami i przypomnieniami.

## Zarządzanie nagrodami
Aplikacja umożliwia użytkownikom wymianę punktów na nagrody. Administratorzy mogą zarządzać nagrodami oraz odebranymi nagrodami. 
### Dodawanie nagrody:
- Administrator może dodawać nowe nagrody poprzez panel administratora.
### Usuwanie nagrody:
- Administrator może usuwać nagrody z listy nagród.
### Zarządzanie odebranymi nagrodami:
- Administrator może przeglądać listę odebranych nagród przez użytkowników oraz usuwać odebrane nagrody, co przywraca punkty użytkownikowi.

## Zarządzanie przypomnieniami
Aplikacja umożliwia użytkownikom dodawanie, edytowanie i usuwanie przypomnień. Administratorzy mogą usuwać przypomnienia innych użytkowników.

## Zarządzanie uprawnieniami
Aplikacja umożliwia administratorom nadawanie i odbieranie ról użytkownikom.
### Nadawanie roli Admin:
- Administrator może przypisać rolę admina użytkownikowi na podstawie adresu e-mail.
### Odbieranie roli Admin:
- Administrator może odebrać rolę admina użytkownikowi na podstawie adresu e-mail.

## Zaawansowane zapytania ORM
Aplikacja korzysta z Entity Framework Core do interakcji z bazą danych. Przykłady zaawansowanych zapytań:
- Agregowanie wyników (np. grupowanie zadań według statusu)
- Filtrowanie (np. wyszukiwanie zadań według daty)
- Łączenie tabel (np. zadania z przypisanymi użytkownikami)

## Przykłady użycia
- Użytkownik tworzy nowe zadanie i przypisuje je sobie.
- Użytkownik oznacza zadanie jako zakończone i otrzymuje punkty.
- Użytkownik wymienia punkty na nagrody.
- Administrator dodaje nowe nagrody do systemu.
- Administrator przegląda i usuwa odebrane nagrody, przywracając punkty użytkownikom.
- Administrator nadaje i odbiera role użytkownikom.
- Administrator usuwa przypomnienia użytkowników.

## Widoki
Aplikacja zawiera różne widoki dla użytkowników i administratorów, które umożliwiają przeglądanie, dodawanie, edytowanie i usuwanie zadań, nagród, przypomnień oraz zarządzanie uprawnieniami.

## Baza danych
Aplikacja wykorzystuje Entity Framework Core do zarządzania bazą danych. Struktura bazy danych obejmuje tabele dla użytkowników, zadań, nagród, przypomnień oraz ról użytkowników.
