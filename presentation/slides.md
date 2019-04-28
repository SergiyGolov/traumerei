---
author: Kim Aurore Biloni & Sergiy Goloviatinski
title: Träumerei
date: 29 Avril 2019
---

# Sommaire

* But
* Démostration
* Architecture
* Génération d'image
* Animation
* Optimisations
* Sauvegarde d'image
* Chargement d'image
* Améliorations possibles

# Träumerei - But

Signifie "rêverie"

Application Xamarin

Génération d'image aléatoire

Gestion images avec "SkiaSharp"

Animation selon l'accéléromètre

# Démonstration

<iframe id="CCJDMAFKB" src="http://192.168.43.1:6868" class="screencasting">
</iframe>

# Architecture

## Xamarin

Algorithme et GUI Multiplaterforme

Interfaces implémentées en code natif pour chargement/sauvegarde image

`DependencyService.get<Interface>()`

# Architecture (2)

## MainPage

* `ImageGenerator_RandomFunctions generator`
* `SKBitmap imgBitmap`

Ajoute les gestionnaires d'événements

Génère une nouvelle image au tap

# Génération d'image

Liste de fonctions de fonctions à 2 variables

```cs
List<Func<Func<double, double, double>, double, double, double>> avalaibleFuncs; //f
(f,x,y)=>Math.Sin(Math.PI*f(x,y)) // exemple
```

Liste de fonctions à 2 variables

```cs
List<Func<double, double, double>> avalaibleAtomicFuncs //af
(x,y)=>x*y // exemple
```
x et y selon axe "mathématique" → pixel au centre de l'image en position (0;0)

# Génération d'image (2)

Et une liste d'opérations possibles

```cs
double z=1;
double localX=x;
double localY=y;
//en parcourant tous les éléments de la liste de fonctions propre 
//à un canal
 switch (comb)
{
    case Combination.Imbricate:
        break;
    case Combination.ReplaceX:
        localX = f(af, x, y);
        break;
    case Combination.ReplaceY:
        localY = f(af, x, y);
        break;
}
z *= f(af, localX, localY);

```

# Génération d'image (3)

Sélection aléatoire de fonctions

*Sinusoïdes, exponentielles, logarithmes, valeur absolue...*

Une liste par canal de couleur avec une taille de liste aléatoire bornée

Les valeurs sont ensuite stockés dans un tableau par canal de couleur

Et finalement assignés au Bitmap

# Animation

Pas d'animation aléatoire par couleur

Récupération des valeurs de l'accéléromètre

Modulo pour pas déborder de l'image

```cs
// For Red Canal
int yR = (y + RYoffset) & sizeBinaryTimes;
int xR = (x + RXoffset) & sizeBinaryTimes;
```

On décale les pixels constituant l'image avec un offset différent selon canal

# Optimisations

Modulo Trick → `a & (n-1) = a % n` <br/>si n puissance de deux

Réduction de la taille de l'image → 2ⁿ < largeur/hauteur d'écran

Arithmétique de pointeur pour couleurs des pixels avec opérations binaires

Multi-threading CPU

# Sauvegarde de l'image

```cs
public interface IPhotoLibrary
{
    Task<Stream> PickPhotoAsync();
    Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
}
```

Implémentation spécifique à Android

Pas implémenté pour IOS

# Sauvegarde de l'image (2)

## Android

Plugin Permissions pour accès au système de fichier

`Environment.DirectoryPictures/Traumerei/`

Ouverture & écriture dans un flux

`MediaScannerConnection.ScanFile(...)`

Vérifie que l'image est bien présentée par la galerie

# Chargement d'une image

```cs
public interface IPhotoLibrary
{
    Task<Stream> PickPhotoAsync();
    Task<bool> SavePhotoAsync(byte[] data, string folder, string filename);
}
```

Aussi permissions d'accès au système de fichier

```Java
MainActivity.Instance.StartActivityForResult(
    Intent.CreateChooser(intent, "Select Picture"),
    MainActivity.PickImageId);
```

Retourne un stream

# Chargement d'une image (2)

Lecture complète du stream

Transformation en SKBitmap

Chargement dans le bitmap principal

Chargement dans classe générateur

# Améliorations possibles

* Possibilité de recadrer image au chargement
* Adapter référence du accéléromètre à position dans main
* Ajouter fonctions disponibles pour génération
* Techniques traitement d'image (amplification quand image toute noire p. ex)
* Rendre la GUI plus sexy

# Sources

* [Reveal.js](https://github.com/hakimel/reveal.js/), Reveal.js Github
* [pandoc.org](https://pandoc.org/index.html), Pandoc for compilation
* [jeremykun.com](https://jeremykun.com/2012/01/01/random-psychedelic-art/), Algorithme
* [reedbeta.com](http://reedbeta.com/blog/generating-abstract-images-with-random-functions/), Algorithme
* [docs.microsoft.com](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/), tutos SkiaSharp
* [github.com/jamesmontemagno](https://github.com/jamesmontemagno/PermissionsPlugin), plugin permissions
* [jacksondunstan.com](https://jacksondunstan.com/articles/1946), modulo trick