# Dziennik podróży

Aplikacja internetowa do zapisywania podróży — wycieczek, wyjazdów. Dodajesz podróże, wpisy z dziennika i wydatki; aplikacja pokazuje podsumowania i raporty.

**Autorzy:** *Alicja Kwiatkowska*

**Stack:** ASP.NET Core MVC, Entity Framework Core, SQLite, sesja (WWW), REST API (login + ApiToken), program konsolowy `ApiClient`.

---

## Uruchomienie (WWW)

```bash
dotnet run
```

Adres: `http://localhost:5297` 

| Konto | Hasło | Rola |
|-------|-------|------|
| admin | 123 | Admin |
| test | haslo | User |

Token REST: po zalogowaniu jako **admin** → menu **Użytkownicy** → kolumna „Token API”.

---

## REST API (demo konsola)

W drugim terminalu (aplikacja WWW musi działać):

```bash
cd ApiClient
dotnet run -- http://localhost:5297 admin WKLEJ_TOKEN
```

Program wykonuje: GET kraje, GET podróże, POST wpis dziennika, GET wpisy.

### Endpointy

| Zasób | URL |
|-------|-----|
| Kraje | `/api/countries` |
| Podróże | `/api/trips` |
| Wpisy | `/api/journalentries` |
| Wydatki | `/api/expenses` |

Metody: **GET, POST, PUT, DELETE**.

---

## Baza danych

**Users** (logowanie — poza limitem 4 tabel): login, hash hasła, rola, ApiToken.

**4 tabele projektu:** Countries, Trips, JournalEntries, Expenses.

Relacje: User → Trips → Country; Trip → JournalEntries, Expenses.

Seed przy pierwszym uruchomieniu: admin, test, kraje, przykładowa podróż.

---

## Funkcje w aplikacji

- CRUD przez WWW: kraje, podróże, wpisy, wydatki
- Logowanie sesją, hash hasła (MD5)
- Admin: lista użytkowników, dodawanie kont
- Raporty: ranking krajów, wydatki wg kategorii, podróże wg dat
