# ReservationApp

ReservationApp to aplikacja webowa umożliwiająca rezerwację usług online. Projekt stworzony z myślą o łatwej obsłudze zarówno dla klientów, jak i usługodawców.

# Główne funkcjonalności

- **Rejestracja i logowanie użytkownika**  
  Rejestracja konta oraz logowanie przy użyciu ASP.NET Identity Core z bezpiecznym przechowywaniem haseł.

- **Zarządzanie kategoriami usług**  
  Możliwość tworzenia, edytowania i usuwania kategorii, do których przypisywani są usługodawcy.

- **Zarządzanie usługodawcami i usługami**  
  Dodawanie usługodawców wraz z przypisaniem ich do kategorii. Usługodawcy mogą definiować dostępne usługi, ich ceny oraz opisy.

- **Przeglądanie i rezerwacja usług**  
  Użytkownicy mogą przeglądać listę dostępnych usług, wybierać dogodny dzień i godzinę.  
  System automatycznie waliduje dostępność terminów, aby zapobiec podwójnym rezerwacjom.

- **Automatyzacja zarządzania rezerwacjami**  
  - Właściciele firm mogą anulować rezerwacje lub oznaczyć je jako *No show*.  
  - Rezerwacje niepotwierdzone przez użytkownika w ciągu 15 minut są automatycznie anulowane przez system.  
  - Wizyty są automatycznie oznaczane jako zrealizowane po ich zakończeniu.

- **Płatności online**  
  Integracja z systemem płatności Stripe.

- **Automatyczne zwroty płatności**  
  W przypadku anulowania wizyty (zgodnie z zasadami anulowania), aplikacja obsługuje automatyczny zwrot środków poprzez Stripe.

- **Historia rezerwacji**  
  Każdy użytkownik może przeglądać swoją historię rezerwacji: zarówno nadchodzące, jak i już zrealizowane wizyty.

- **Powiadomienia e-mail**  
  Automatyczne wysyłanie powiadomień e-mail o zbliżających się rezerwacjach oraz o zmianach statusu rezerwacji, realizowane przy użyciu Hangfire oraz SMTP.

- **Recenzje i oceny usługodawców**  
  Po skorzystaniu z usługi, użytkownik może wystawić ocenę (w formie gwiazdek) oraz dodać opinię tekstową dla usługodawcy.

- **Generowanie raportów**  
  Administratorzy mogą generować raporty rezerwacji z wybranego okresu czasu. Raporty mogą być eksportowane do plików CSV przy użyciu CsvHelper.

- **Rejestrowanie logów aplikacji**  
  Pełne logowanie działania aplikacji, obsługi błędów i ważnych operacji biznesowych przy pomocy Serilog.




## Tech Stack

- **Backend**: C#, ASP.NET Core 6, Entity Framework Core
- **Frontend**: Razor Pages (cshtml) + Bootstrap v5.3
- **Baza danych**: MS SQL Server
- **Autoryzacja**: ASP.NET Identity Core
- **Płatności**: Stripe API
- **Logowanie**: Serilog
- **Zadania w tle**: Hangfire
- **Eksport danych**: CsvHelper

---

## Instalacja lokalna
 Sklonuj repozytorium:
   ```bash
   git clone https://github.com/mieczkowski107/reservationapp.git
   cd reservationapp
   dotnet ef database update
   dotnet run
   ```
   > Upewnij się, że masz zainstalowane .NET 6 SDK oraz poprawnie skonfigurowane połączenie do bazy danych w pliku appsettings.json.
   Aplikacja będzie dostępna pod adresem: https://localhost:7038/

