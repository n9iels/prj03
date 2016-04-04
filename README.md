# HR Project 3
In deze repository is alle code van Project 3 te vinden. Hieronder staat een handleiding over het werken met deze repository

## Repository naar je pc halen
1. Open Git op je pc in de map waar de code moet komen en vul het volgende commando in `git clone https://github.com/n9iels/prj03.git .` Druk op enter om het commando uit te voeren.

## Wijzigingen doorvoeren
1. Ga naar de repository, klik op het selectieveld "Branch", type een gewenste naam en klik op "Create branch". <b>Let op:</b> Voer deze stap altijd uit als je met iets nieuws begint! Je wilt niet direct vanuit de master werken.
2. Open op je pc Git door met de rechter muisknop op de project map te klikken, en dan te selecteren "Git bash"
3. Voer de volgende commando's los van elkaar uit: `git pull` `git checkout ...` vul in plaats van de ... de naam van de branch in
4. Maak nu alle wijzigingen die je wilt maken
5. Open als je klaar bent met je wijzigingen open je Git weer op je pc. Voer daarna volgende commando's na elkaar uit `git add --all` `git commit -m "samenvatting van de PR"` `git push`
6. Ga naar GitHub en selecteer de branch in het selectieveld, er staat nu een groene button dat de branch voorloopt op de originele branch. Klink op de groene "Compare" knop en daarna op de Groene "Create Pull Request" knop
7. Vul een titel en opschrijving in en druk wederom op "Create pull request". Je wijzigingen zijn nu ingedient
8. Indien je na het aanmaken van de PR nog wijzigingen wil maken hieraan, kan dit door simpelweg door een wijzigingen te pushen naar de branch. Deze worden dan automatisch opgenomen in de PR.

## Bijwerken
Op de laatste vesie van het project naar je pc te halen ga naar de branch develop met het command `git checkout develop`. Hier zeg je `git pull` om je branch bij te werken.

## Regels en gebruiken
- Er wordt door niemand direct in de branch Master gewerkt. Deze wordt enkel up-to-date gemaakt op de laatste dag van de sprint
- Een PR wordt pas gemerged als alle groepsleden deze hebben gezien en hun goedkeuring hebben gegeven
- Voor elke code gerelateerde User Story wordt een issue aangemaakt op GitHub. Als je deze User Story oppakt assign je deze issue ook aan jezelf op GitHub. Bij het commiten en maken van een PR vermeld je ook altijd het issue nummer.

## Wachtwoord onthouden
Gebruik het volgende commando om je wachtwoord door Git 15 minuten te laten onthouden `git config --global credential.helper wincred`
