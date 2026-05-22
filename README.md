# Dziennik podróży

Aplikacja internetowa do zapisywania podróży — wycieczek, wyjazdów. Dodajesz podróże, wpisy z dziennika i wydatki, a aplikacja pokazuje podsumowania: ile wydałaś, które kraje odwiedziłaś najczęściej, średnie oceny miejsc itd.

**Stack:** ASP.NET Core MVC, Entity Framework, SQLite, logowanie przez sesję, REST API (login + token).


---

## Do czego to służy

- **Podróże** — zapisujesz gdzie byłaś, kiedy (data od–do), krótki opis, przypisany kraj.
- **Dziennik** — wpisy z datą, tytułem, treścią i oceną miejsca od 1 do 5.
- **Wydatki** — kwota, co to było, data, kategoria (jedzenie, transport, nocleg…).
- **Raporty** — nie tylko listy tabel: suma wydatków, ranking krajów, podróże wg dat, statystyki wpisów.

Żeby cokolwiek zobaczyć, trzeba się **zalogować**. Przy pierwszym uruchomieniu powstaje konto **admina** — tylko on może dodawać nowych użytkowników i przeglądać listę kont w systemie.

---

## Baza danych — co jest w tabelach

### Users

| Pole / sens | Co tam jest |
|-------------|-------------|
| Login | Nazwa do logowania |
| Hasło | Hash |
| Rola | `Admin` albo zwykły `User` |
| ApiToken | Klucz do REST API — każdy user ma swój |

### Tabele projektu

| Tabela | Co przechowuje |
|--------|----------------|
| **Countries** | Kraje — nazwa, opcjonalnie kod kraju (np. PL, IT) |
| **Trips** | Podróże — tytuł, data od, data do, opis; do którego **kraju** i którego **usera** należy |
| **JournalEntries** | Wpisy dziennika — data, tytuł, treść, ocena 1–5; należy do jednej **Trip** |
| **Expenses** | Wydatki — kwota, opis, data, kategoria; należy do jednej **Trip** |

### Jak to się łączy

```
User 1 ── * Trip * ── 1 Country
                 │
                 ├── 1 ── * JournalEntry
                 │
                 └── 1 ── * Expense
```

Jeden user ma wiele podróży. Jedna podróż ma jeden kraj, wiele wpisów i wiele wydatków.

Przy **pierwszym starcie** aplikacja sama zakłada bazę, admina i trochę przykładowych danych (kraje, podróż, wpisy, wydatki) — żeby od razu coś było widać.

---

## Widoki „extra” (nie zwykła tabela)

| Widok | Po co |
|-------|--------|
| **Dashboard** | Statystyki: ile podróży, suma wydatków, średnia ocena, ostatnie wpisy |
| **Ranking krajów** | Który kraj ile razy, ile wydatków, jaka średnia ocena |
| **Podróże wg dat** | Nadchodzące, trwające, zakończone |

Reszta to normalny CRUD: kraje, podróże, wpisy, wydatki — wszystko z menu, bez wpisywania URL ręcznie.

---

## Uruchomienie

```bash
cd lab10-12
dotnet run
```

Konto admina po seedzie: **admin** / **123** (oraz test / haslo)

---
