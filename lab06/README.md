# Лаборатобна робота №6: Підміна віддаленого серверу

Запустимо програму victim:
java -jar .\victim.jar
Reading plaintext data from external resources:
Davydov Viacheslav
CS 261: Systems Security
DecryptedText: Hello, world

![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab06/victim1.png?raw=true)

# Шаг 1. Визначення компонентів підміни. 
Для виконання роботи нам по-перше доведеться визначити спосіб виведення інформаціх додатком  victim.jar. Є декілька способів це зробити, 
а) Встановив додаток для низькорівневого перехвату трафіку (наприклад Wireshark) та під час запуску додатку victim відстежити пакети які відправляє додаток (там можна дізнатись destination ip / hostname, url та навіть відповідь, частина якої викрористувується для відображення інформації.
б) скористуватись декомпілятором java (наприклад Java Decompiler) та переглянути код додатку. (Цей спосіб швидше)

Згідно з докомпільованих сорсів ми бачимо 2 ключових виклики:
1) Для отримання значення імені та фамілії:
String userDetails = getMessage("https://api.github.com/users/davydov-vyacheslav");
JSONObject jsonObject = new JSONObject(userDetails);
return jsonObject.getString("name");

2) Для отримання значення заголовку
String webpage = getMessage("http://128.32.244.190/~raluca/cs261-f15/");
return webpage.substring(webpage.toLowerCase().indexOf("<title>\n") + "<title>\n".length(), webpage.toLowerCase().indexOf("</title>")).trim();

Для зміни стрічки з іменем нам потрібно зробити так щоб виклик "https://api.github.com/users/davydov-vyacheslav" віддав json з потрібнем полем name.
Для зміни стрічки з назвою дісциплини потрібно підминити виклик "http://128.32.244.190/~raluca/cs261-f15/" так щоб він віддав HTML з заголовком TITLE = Сучаснi Технологii Безпечного Програмування

# Конфігурування підміни
Утіліта stubby4j дозволяє нам підмінити відповідь на заклик, с заданим url, header-ами та іншими параметрами. Тож ми скористуємой єю.
Створимо конфігураційний файл hw.yaml:

-  request:
      method: GET
      url: /~raluca/cs261-f15/

   response:
      body: "<HTML><HEAD><TITLE>Сучасні технології безпечного програмування</TITLE></HEAD></HTML>"


-  request:
      method: GET
      url: /users/davydov-vyacheslav

      
   response:
      headers:
        content-type: application/json
      body: "{name:'Kostiantyn Lazurenko'}"

![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/config.jpg?raw=true)

Та запустимо stubby:
java -jar .\stubby4j-7.5.2.jar -d .\hw.yaml -s 80

Якщо на данному етапі запустити утіліту і відкрити browser то можно протестувати нашу конфігурацію:
http://localhost/users/davydov-vyacheslav
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/stubby_test1.png?raw=true)

http://localhost/~raluca/cs261-f15/
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/stubby_test2.jpg?raw=true)

# Проксіювання трафіку на сервер заглушок Stubby

Наступна задача буде перехопити трафік додатку (victim) та модифікувати його так щоб він відправляв запити на наш Stubby4j сервер - localhost (80 порт). Э різні додатки для цього, зокрема fiddler, яким я і скористуюсь (адже мав із ним досвіт).

Fiddler працює як реверсивний проксі, тобто зовнішні виклики він направляє спочатку назад на localhost (порт за умовчанням 8888) під яким він и перехоплює трафік. Але за умовчанням фідлер перехоплює лише трафік браузеров (chrome, edge...) тож для того щоб трафік з нашего аплікейшну victim був перехоплений та скорегований fiddler-ом потрібно зробити деякі конфігураційні зміни:
1. Запустимо fiddler та зайдемо в налаштування Tools -> Options, а там виберемо вкладку HTTPS. Оскільки ми будемо мати справу з SSL з'єднаннями то потрібно Там потрібно включити параметри "Capture HTTPs CONNECTs" та "Decrypt HTTPs traffic", також сертифікат fiddler потрібно встановити в систему - для цього вибрати Actions -> Trust Root Certificate і далі експортувати його Actions -> Export Root Certificate to Desktop.
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/fiddler_https.jpg?raw=true)
2. Далі потрібно зайти в меню налаштувань Connections та впевпетись що наступні параметри визначені наступним чином, порт для прослуховання - 8888 (впринципі його можно змінити але ми не будемо цього робити), та "Allow remote computers to connect" включено.
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/fiddler_connections.jpg?raw=true)

3. Після цього доцільно перезапустити Fiddler аби впевнетись що зміни вступили в силу.

4. Далі потрібно налаштувати додатки Java аби виклики з тих проходили через fiddler. Скористуэмось інструкцією з офіційного сайту telerik fiddler:

Поперше імпортуємо сертифікат fiddler (який ми раніше експортували з налаштувань HTTPs) за допомогою утіліти keytool
<JAVA_HOME>\bin>keytool.exe -import -file C:\Users\Kostiantyn_Lazurenko\Desktop\FiddlerRoot.cer -keystore c:\Users\Kostiantyn_Lazurenko\Desktop\FiddlerKeystoreFile -alias Fiddler

Далі підключимо цей сертифікат та задамо конфігурацию проксі (порт повинен відповідати порту заданому в Fiddler->tools->options->connections):
bin>javaw.exe -DproxySet=true -DproxyHost=127.0.0.1 -DproxyPort=8888 -Djavax.net.ssl.trustStore=C:\Users\Kostiantyn_Lazurenko\Desktop\FiddlerKeystoreFile -Djavax.net.ssl.trustStorePassword=<password>

Це дозволить вихідний трафік Java додатків направляти через fiddler.

5. Тепер fiddler, маючий цей трафік можна сконфугурувати так щоб він виконував нашу мету - перенаправляв запроси "https://api.github.com/users/davydov-vyacheslav" к "https://localhost/users/davydov-vyacheslav" а запроси "http://128.32.244.190/~raluca/cs261-f15/" к "http://localhost/~raluca/cs261-f15/". Для цього можно скористуватись або FiddlerScript, або AutoResponder, в якому ми маємо задати наступні правила:
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/fiddler_autoresponder.jpg?raw=true)

Після цього нам потрібно впевнетись що ці правила включені (AutoResponder -> Enable rules).

Наразі перевіримо результат запустив додаток victim:
![alt text](https://github.com/Konstantin86/SecuredProgramming_KostiantynLazurenko/blob/main/lab08/victim2.png?raw=true)