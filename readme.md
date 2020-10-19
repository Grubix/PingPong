# Wrzesień
## 16
(WM,BB) zapoznanie sie z robotem, układami współrzędnych, rodzaje ruchu, KRC, kalibracja narzędzia, kalibracja układu bazowego, ruch po punktach (w przestrzeni, po trójkącie, po okręgu, zabieranie kostki z wieżyczki i wrzucanie jej na górę)

## 17
przekazywanie wiedzy Krystianowi, ustawianie kostek na stole (2 petle, programowanie w expert modzie)

## 18
rozwiązanie problemu osobliwości (zmiana orientacji narzędzia by uniknąć gwałtownego ruchu), wykorzystanie inputów (przełączniczków) do sterowania robotem (wybór 1 z 9 pól do postawienia kloca wraz z zabezpieczeniem przed kolizją)

## 24
analiza i rozpoznanie zadań (rezygnacja z pisania w Javce[brak SDK], rezygnacja pisania w vsCode [brak buildera GUI], zaktualizowanie licencji Visuala), przemyślenie koncepcji i funkcjonalności które mogą być zaimplementowane lub poprawione, rozmieszczenie uchwytów do OptiTracka

## 30
RSI - połączenie kuka-PC z programem testowym

# październik
## 5
próba połączenia kuki z czymś innym niż example RSI, udało się nasłuchiwać, problem przy wysyłaniu na konkretny adres, komunikacja z Resilio (pusta ramka)
[btw: IP kuki: 192.168.3.2, IP kompa: 192.168.3.1, port: 8081]

## 7
Wysyłanie ruchu do robota (nie-smooth), zwiększenie ograniczeń w rsix do 5000 by się swobodniej poruszać

## 9
Przykręcenie optiTracka i wstępne połączenie się z nim

## 12
Poprawne wygenerowanie trajektorii ruchu (xyz), locki

## 16
Ustawienie ograniczeń w programie (na podstawie dojechania do pozycji), praca nad trajektorią ABC

# Info

## Sieć komputerowa
Zbiór komputerów i innych urządzeń połączonych z sobą kanałami komunikacyjnymi oraz oprogramowanie wykorzystywane w tej sieci. Umożliwia ona wzajemne przekazywanie informacji oraz udostępnianie zasobów własnych między podłączonymi do niej urządzeniami, zwanymi punktami sieci.

## Adres IP
(ang. Internet Protocol address – IP address) to unikatowy numer przyporządkowany urządzeniom w sieciach komputerowych, protokołu IP. Adresy IP są wykorzystywane w Internecie oraz sieciach lokalnych. Adres IP zapisywany jest w postaci czterech oktetów w postaci dziesiętnej oddzielonych od siebie kropkami.

W adresach, które zostały przypisane komputerom, część bitów znajdująca się z lewej strony 32-bitowego adresu IP identyfikuje sieć. Liczba tych bitów zależy od tzw. klasy adresu. Pozostałe bity w 32-bitowym adresie IP identyfikują konkretny komputer znajdujący się w tej sieci. Taki komputer nazywany jest hostem. Adres IP komputera składa się z części sieciowej i części hosta, które reprezentują konkretny komputer znajdujący się w konkretnej sieci.

Aby poinformować komputer o sposobie podziału na części 32-bitowego adresu IP, używana jest druga 32-bitowa liczba, zwana maską podsieci. Maska ta wskazuje, w jaki sposób powinien być interpretowany adres IP, określając liczbę bitów używanych do identyfikacji sieci, do której jest podłączony komputer. Maska podsieci jest wypełniana kolejnymi jedynkami wpisywanymi od lewej strony maski. Maska podsieci będzie zawierała jedynki w tych miejscach, które mają być interpretowane jako adres sieci, a pozostałe bity maski aż do skrajnego prawego bitu będą równe 0. Bity w masce podsieci równe 0 identyfikują komputer lub hosta znajdującego się w tej sieci.

### Przykłady masek podsieci:
* 11111111000000000000000000000000 zapisana w notacji kropkowo-dziesiętnej jako 255.0.0.0 lub
* 11111111111111110000000000000000 zapisana w notacji kropkowo-dziesiętnej jako 255.255.0.0

\
W pierwszym przykładzie pierwsze osiem bitów od lewej strony reprezentuje część sieciową adresu, natomiast pozostałe 24 bity reprezentują część adresu identyfikującą hosta. W drugim przykładzie pierwsze 16 bitów reprezentuje część sieciową adresu, a pozostałe 16 bitów reprezentuje część adresu identyfikującą hosta.

## Socket (gniazdo)
pojęcie abstrakcyjne reprezentujące dwukierunkowy punkt końcowy połączenia. Dwukierunkowość oznacza możliwość wysyłania i odbierania danych. Wykorzystywane jest przez aplikacje do komunikowania się przez sieć w ramach komunikacji międzyprocesowej.

Socket posiada trzy główne właściwości:
* typ gniazda identyfikujący protokół wymiany danych
* lokalny adres (np. adres IP, IPX, czy Ethernet)
* opcjonalny lokalny numer portu identyfikujący proces, który wymienia dane przez gniazdo (jeśli typ gniazda pozwala używać portów)
* **Z TEGO CO ROZUMIEM TO UdpClient TO JEST SOCKET KTÓRY DO KOMUNIKACJI WYKORZYSTUJE PROTOKÓŁ UDP**

Adres IP wyznacza węzeł w sieci\
Numer portu określa proces w węźle\
Typ gniazda determinuje sposób wymiany danych (np. protokuł TCP, UDP)\