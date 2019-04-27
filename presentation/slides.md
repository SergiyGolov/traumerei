---
author: Kim Aurore Biloni & Sergiy Goloviatinski
title: Träumerei
date: 29 Avril 2019
---

# Table des matières

## TODO

# Träumerei

Signifie "rêverie"

Application Xamarin

Génération d'image aléatoire

Animation selon l'accéléromètre

# Démonstration

<iframe id="CCJDMAFKB" src="http://192.168.43.1:6868" class="screencasting">
</iframe>

# Architecture

## Xamarin

Multiplaterforme

Interfaces implémentées en code natif

`DependencyService.get<Interface>()`

# Architecture (2)

## MainPage

* `ImageGenerator_RandomFunctions generator`
* `SKBitmap imgBitmap`

Ajoute les gestionnaires d'événements

Génère une nouvelle image au tap

# Génération d'image

Utiliation d'une liste de fonctions de fonctions de double

```cs
List<Func<Func<double, double, double>, double, double, double>> avalaibleFuncs
```

Et d'une autre avec atomic

```cs
List<Func<double, double, double>> avalaibleAtomicFuncs
```

# Génération d'image (2)

Sélection aléatoire de fonctions à deux variables

*Sinusïdes, exponetielles, logarithmiques,...*

Une par canal de couleur

Les valeurs sont ensuites assignées au Bitmap

# Animations

Pas d'animation pas couleur

Récupération des valeurs de l'accéléromètre

Modulo Trick

```cs
// For Red Canal
int yR = (y + RYoffset) & sizeBinaryTimes;
int xR = (x + RXoffset) & sizeBinaryTimes;
```

Regénération de l'image

# Optimisations

Modulo Trick → le binaire est plus rapide

Réduction de la taille de l'image → 2ⁿ

```cs
int threadNb = Environment.ProcessorCount;
Thread[] threads = new Thread[threadNb];
```

Génération parrallèle

# Sauvegarde de l'image

--- c'était l'enfer non de non ----

# Chargement d'une image

--- Ici aussi certainement ---

# Améliorations

* Qu'est-ce
* qui
* est
* mauvais

# Sources

* [Reveal.js](https://github.com/hakimel/reveal.js/), Reveal.js Github
* [icons8.com](https://icons8.com/icon/set/zoom-3d/nolan), If you need some icons
* [pandoc.org](https://pandoc.org/index.html), Pandoc for compilation
* [other](/todo)
