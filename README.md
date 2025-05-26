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
## Pogląd działania
### Strona główna
![Zrzut ekranu 2025-05-26 212606](https://github.com/user-attachments/assets/fb634a72-3aa2-4d6d-b87f-f808b8129766)

### Przeglądanie usług oraz recenzji danego usługodawcy
![Zrzut ekranu 2025-05-26 212658](https://github.com/user-attachments/assets/274319f3-4cfd-49a8-ac77-75f776003857)
![Zrzut ekranu 2025-05-26 212710](https://github.com/user-attachments/assets/a1b10376-1209-402b-951e-e6e3e76edf98)

### Proces rezerwazji

![Zrzut ekranu 2025-05-26 212128](https://github.com/user-attachments/assets/4759f02b-fae4-4ed7-9a6b-4c0a07812b9b)
![Zrzut ekranu 2025-05-26 212134](https://github.com/user-attachments/assets/28de8397-b13a-4c47-9f74-60ba0f4534e5)
![Zrzut ekranu 2025-05-26 212202](https://github.com/user-attachments/assets/704dbb81-860e-4232-8df8-71d8d216add7)

### Przeglądanie i zarządzanie rezerwacjami

![Zrzut ekranu 2025-05-26 212117](https://github.com/user-attachments/assets/d8beac7e-e31c-44a1-9a61-3a1b86b76e15)

### Zarządzanie usługodawcami 
![Zrzut ekranu 2025-05-26 212233](https://github.com/user-attachments/assets/e680435b-6046-4d60-87b8-d25e8a09eaae)

### Zarządzanie rezerwazjami przez usługodawce
![Zrzut ekranu 2025-05-26 212107](https://github.com/user-attachments/assets/01456d3f-6131-42c4-90bc-e33504e681a3)
