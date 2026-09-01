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

