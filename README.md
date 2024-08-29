# Stopwatch Web API

Opće funkcionalnosti: registracija i login korisnika, upravljanje štopericom (pokretanje, zaustavljanje, resetiranje) te pregled sesija štoperice (za jednog korisnika ili sve).

## Korištene Tehnologije

FastEndpoints, Entity Framework Core, ASP.NET Core

## Baza podataka

U bazi se nalaze tri tablice: Users, UserRoles (role pridodjeljene userima) i TimedSessions (sesije štopanja za svakog „participant-a“).

## Značajke

### Registracija

- **Endpoint**: `POST  api/register`
- **Opis**: Registracija novog korisnika.Endpoint prima korisničke podatke (ime, prezime, email, lozinka) i stvara novog korisnika u sustavu.

### Prijava

- **Endpoint**: `POST api/login`
- **Opis**: Prijava postojećeg korisnika. Endpoint validira korisničke podatke i vraća JWT token za autentifikaciju.

### Upravljanje Korisnicima

- **GetUsers**
   - **Endpoint**: `GET api/users`
   - **Opis**: Dohvaća popis svih korisnika registriranih u sustavu. Može pristupiti samo administrator.

- **GetUser**
   - **Endpoint**: `GET api/users/{id}`
   - **Opis**: Dohvaća detalje o određenom korisniku prema njegovom ID-u. Može pristupiti samo administrator.

- **UpdateUser**
   - **Endpoint**: `PUT api/users/{Id}}`
   - **Opis**: Ažurira informacije o korisniku prema dostavljenim podacima. Može pristupiti samo administrator.

- **RemoveUser**
   - **Endpoint**: `POST api/users/delete/{id}`
   - **Opis**: Briše korisnika iz sustava. Može pristupiti samo administrator.

### Upravljanje Ulogama

- **AddRole**
   - **Endpoint**: `POST api/users/addrole`
   - **Opis**: Dodaje ulogu određenom korisniku. (administrator/participant) Može pristupiti samo administrator.

- **RemoveRole**
   - **Endpoint**: `POST api/users/removerole`
   - **Opis**: Uklanja ulogu od korisnika. Može pristupiti samo administrator.

### Upravljanje Štopericom

Mogu pristupiti samo participants.

- **StartStopwatch**
   - **Endpoint**: `PUT api/stopwatch/start`
   - **Opis**: Pokreće novu ili nastavlja postojeću neresetiranu sesiju štoperice za trenutno prijavljenog korisnika.

- **EndStopwatch**
   - **Endpoint**: `POST api/stopwatch/stop`
   - **Opis**: Zaustavlja aktivnu sesiju štoperice za trenutno prijavljenog korisnika. Ponovnim pokretanjem i zaustavljanjem mijenja se završno vrijeme sesije (nastavlja se štopanje dok nije pokrenuto resetiranje štoperice).

- **ResetStopwatch**
   - **Endpoint**: `POST api/stopwatch/reset`
   - **Opis**: Resetira aktivnu sesiju štoperice za trenutno prijavljenog korisnika, pripremajući je za ponovno pokretanje.

- ** GetMyTimedSessions**
   - **Endpoint**: `GET api/stopwatch/my-timed-sessions`
   - **Opis**: Dohvaća sve sesije štoperice za trenutno prijavljenog korisnika, njihovo pojedinačno proteklo vrijeme i ukupno proteklo vrijeme.

- ** GetAllTimedSessions**
   - **Endpoint**: `GET api/stopwatch/all-timed-sessions`
   - **Opis**: Dohvaća sve sesije štoperice za sve korisnike, uključujući ime i prezime korisnika te protekla vremena sesija.