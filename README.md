# BeerStockApi
ASP.NET Core REST API for brewery inventory and quote management.

## Prérequis

- [.NET SDK 10](https://dotnet.microsoft.com/download)

## Démarrage du projet

```powershell
# Restaurer les dépendances
dotnet restore

# Lancer l'API (depuis la racine du repo)
dotnet run --project src/BeerStockApi
```

L'API démarre sur `http://localhost:5101` et ouvre automatiquement Swagger (`/swagger`).

Au premier démarrage, la base SQLite (`beerstock.db`) est créée automatiquement via les migrations EF Core, puis peuplée avec des données de test (voir ci-dessous). Les démarrages suivants ne re-seedent pas si des bières existent déjà.

## Démonstration HTTP

Le fichier [BeerStockApi.http](src/BeerStockApi/BeerStockApi.http) contient un scénario complet pour tester les fonctionnalités avec des requêtes HTTP :

- consulter les bières, les brasseries et les grossistes ;
- créer et modifier une bière ;
- ajouter une bière au catalogue d'un grossiste ;
- consulter et mettre à jour le stock ;
- générer un devis sans remise, avec 10 % de remise et avec 20 % de remise ;
- vérifier les erreurs de commande vide, de doublon, de grossiste inexistant, de quantité invalide, de stock insuffisant et de bière absente du catalogue du grossiste ;
- vérifier les validations de stock : bière déjà présente dans le catalogue et quantité négative ;
- supprimer la bière créée pour la démonstration.

### Exécuter les requêtes dans VS Code

1. Installer l'extension [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client).
2. Démarrer l'API depuis la racine du dépôt :

	```powershell
	dotnet run --project src/BeerStockApi
	```

3. Ouvrir [BeerStockApi.http](src/BeerStockApi/BeerStockApi.http).
4. Exécuter les requêtes dans l'ordre en cliquant sur **Send Request** au-dessus de chaque bloc.

Chaque bloc de requête est séparé par `###`. La réponse s'affiche dans un nouvel onglet VS Code.

La démonstration utilise les données initiales suivantes : `brewerId = 1`, `wholesalerId = 1` et `beerId = 1`. La bière créée reçoit normalement l'ID `11` sur une base neuve. Après une nouvelle exécution, reporter l'ID retourné par la requête de création dans la variable `createdBeerId` avant de poursuivre le scénario.

La requête de suppression est placée en dernier et est volontairement destructive. Elle peut être ignorée si l'on souhaite conserver la bière créée.

## Données de test (seed)

Le seed est défini dans [DbInitializer.cs](src/BeerStockApi/Infrastructure/DbInitializer.cs) et inclut :
- 7 brasseurs (Abbaye de Leffe, Duvel Moortgat, Chimay, Orval, Rochefort, Achouffe, Koningshoeven)
- 3 grossistes (GeneDrinks, Belgian Beer Supply, Ardennes Drinks)
- 10 bières réparties entre les brasseurs, avec un stock initial chez un grossiste chacune

Pour repartir d'une base vierge, supprimez le fichier `src/BeerStockApi/beerstock.db` puis relancez l'API.

## Tests

```powershell
dotnet test
```

