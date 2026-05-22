# Dziennik podróży

Aplikacja internetowa do zapisywania studenckich podróży — wycieczek, wyjazdów i weekendów. Użytkownik dodaje podróże, wpisy z dziennika, wydatki i tagi, a aplikacja pokazuje podsumowania (koszty, ranking krajów, statystyki wpisów).

**Technologie:** ASP.NET Core MVC, Entity Framework Core, SQLite, sesja (logowanie), REST API (autentykacja login + token API).

**Autorzy:** *(uzupełnij imiona i nazwiska)*

---

## Do czego służy aplikacja

- Zapisywanie podróży powiązanych z krajem (daty, opis).
- Prowadzenie dziennika — wpisy z datą, treścią i oceną miejsca (1–5).
- Ewidencja wydatków w podróży (kwota, kategoria, data).
- Tagowanie wpisów (np. plaża, muzeum, jedzenie).
- Raporty: suma wydatków, średnie oceny, ranking krajów, podróże wg dat.

Dostęp do widoków tylko po zalogowaniu. Administrator (konto z seeda przy pierwszym uruchomieniu) dodaje użytkowników i przegląda listę kont.

---

## Baza danych — tabele

### Tabele logowania (nie wliczają się do wymaganego minimum 4 tabel projektu)

| Tabela | Zawartość |
|--------|-----------|
| **Users** | Login, hash hasła, rola (User / Admin), unikalny **ApiToken** do REST API |

### Tabele domenowe (projekt)

| Tabela | Zawartość |
|--------|-----------|
| **Countries** | Nazwa kraju, opcjonalnie kod (np. PL, IT) |
| **Trips** | Tytuł podróży, data od / do, opis; powiązanie z **Country** i właścicielem (**User**) |
| **JournalEntries** | Wpis dziennika: data, tytuł, treść, ocena 1–5; należy do jednej **Trip** |
| **Expenses** | Wydatek: kwota, opis, data, kategoria (np. jedzenie, transport, nocleg); należy do **Trip** |
| **Tags** | Nazwa tagu (unikalna), np. `plaża`, `muzeum` |
| **EntryTags** | Powiązanie wiele-do-wiele: który **JournalEntry** ma który **Tag** |

### Relacje

```
User 1 ── * Trip * ── 1 Country
                 │
                 ├── 1 ── * JournalEntry * ── * EntryTag * ── 1 Tag
                 │
                 └── 1 ── * Expense
```

Przy **pierwszym uruchomieniu** aplikacja tworzy bazę, konto administratora oraz przykładowe kraje, podróże, wpisy i wydatki.

---

## Funkcjonalność (plan)

| Obszar | Opis |
|--------|------|
| MVC | CRUD dla krajów, podróży, wpisów, wydatków, tagów; menu nawigacyjne |
| Sesja | Logowanie / wylogowanie; ochrona widoków |
| Admin | Dodawanie użytkowników, lista kont |
| Raporty | Podsumowania kosztów, ranking krajów, statystyki wpisów |
| REST API | GET/POST/PUT/DELETE z nagłówkami lub parametrami login + ApiToken |
| Konsola | Program(y) demonstrujące wywołania API |

---

## Uruchomienie

*(uzupełnisz po utworzeniu projektu ASP.NET)*

```bash
cd lab10-12
dotnet run
```

Domyślne konto administratora (seed): *(login / hasło — uzupełnij w kodzie lub tutaj po implementacji)*

---

## Dokumentacja projektu

Pełny opis funkcji i sposobu użycia: ten plik + *(opcjonalnie strona /Docs w aplikacji)*.
