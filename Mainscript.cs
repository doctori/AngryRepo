///////////////////////////////////////////////////
///// Projet         : ROetIA-PathAlgo	     //////
///// Auteur         : Crew ACPL		     //////
///// Derni�re Modif : 2013-12-01            //////
///////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;


/// <summary>
/// Classe principale � utiliser pour impl�menter vos algorithmes
/// Si vous souhaitez utiliser plusieurs scripts (1 par algorithme), 
/// vous le pouvez aussi.
/// </summary>
public class MainScript : MonoBehaviour
{

    /// <summary>
    /// Indique si un algorithme est en cours d'ex�cution
    /// </summary>
    private bool _isRunning = false;
	private bool _wasTrue = false;
    /// <summary>
    /// Indique si une evaluation de solution est en cours
    /// </summary>
    private bool _inSimulation = false;
	
	
	private String _csvLog;
    /// <summary>
    /// M�thode utilis�e pour g�rer les informations et 
    /// boutons de l'interface utilisateur
    /// </summary>
    public void OnGUI()
    {
        // D�marrage d'une liste de composants visuels verticale
        GUILayout.BeginVertical();

        // Affiche un bouton permettant le lancement de la recherche locale na�ve
        if (GUILayout.Button("DEMARRAGE RECHERCHE LOCALE NAIVE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de la recherche locale na�ve en pseudo asynchrone
                StartCoroutine("NaiveLocalSearch");
            }
        }

        // Affiche un bouton permettant le lancement de la recherche locale na�ve
        if (GUILayout.Button("DEMARRAGE RECUIT SIMULE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage du recuit simul� en pseudo asynchrone
                StartCoroutine("SimulatedAnnealing");
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme g�n�tique
        if (GUILayout.Button("DEMARRAGE ALGORITHME GENETIQUE"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme g�n�tique en pseudo asynchrone
             StartCoroutine(StartGenetic());
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme de Djikstra
        if (GUILayout.Button("DEMARRAGE DJIKSTRA"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme de Djikstra en pseudo asynchrone
                StartCoroutine("Djikstra");
            }
        }

        // Affiche un bouton permettant le lancement de l'algorithme A*
        if (GUILayout.Button("DEMARRAGE A*"))
        {
            // Le bouton est inactif si un algorithme est en cours d'ex�cution
            if (!_isRunning)
            {
                // D�marrage de l'algorithme A* en pseudo asynchrone
                StartCoroutine("AStar");
            }
        }

        // Fin de la liste de composants visuels verticale
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Initialisation du script
    /// </summary>
    void Start()
    {
        // Pour faire en sorte que l'algorithme puisse continuer d'�tre actif m�me
        // en t�che de fond.
        Application.runInBackground = true;
    }
public IEnumerator StartGenetic()
	{
	File.AppendAllText(@"log.csv", "Numero test;Fitness;Population Initiale;Taux de mutation;Temps;Nb Iterations;Correct/Opti/Echec;\n");
		//On defini nos variable pour variation Avec un pas (peu etre negatif)
		float fitness = 0.1f;
		float fitnessPas = 0f;
		float mutation = 0.1f;
		float mutatonPas = 0f;
		int mutationSequence = 1;
		int mutationSequencePas = 1;
		int	population = 60;
		int populationPas = 0;
		for(int i =0;i<10;i++){
			_wasTrue = false;
				Debug.Log("------------");
			Debug.Log("Test numero : "+i);
			Debug.Log("Valeur fitness : "+fitness);
			Debug.Log("Valeur mutation : "+ mutation);
			Debug.Log("Valeur Population : "+population);
			_csvLog = i+";"+fitness+";"+population+";"+mutation+";";
		
					//on ajoute d'un pas a chauqe iteration g�n�rale
			yield return StartCoroutine(GeneticAlgorithm(population,mutation,fitness,mutationSequence));		
					fitness = fitness +fitnessPas;
					mutation = mutation +mutatonPas;
					population = population + populationPas;
		
					}
	}
	
    
    /// <summary>
    /// Impl�mentation possible de la recherche locale na�ve
    /// sous forme de coroutine pour le mode pseudo asynchone
    /// </summary>
    /// <returns></returns>
    public IEnumerator NaiveLocalSearch()
    {
        // Indique que l'algorithme est en cours d'ex�cution
        _isRunning = true;
		DateTime time = DateTime.Now;
        // G�n�re une solution initiale au hazard (ici une s�quence
        // de 42 mouvements)
        var currentSolution = new PathSolutionScript(42);
		    int iterations = 0;

        // R�cup�re le score de la solution initiale
        // Sachant que l'�valuation peut n�cessiter une 
        // simulation, pour pouvoir la visualiser nous
        // avons recours � une coroutine
        var scoreEnumerator = GetError(currentSolution,time,iterations);
        yield return StartCoroutine(scoreEnumerator);
        float currentError = scoreEnumerator.Current;

        // Nous r�cup�rons l'erreur minimum atteignable
        // Ceci est optionnel et d�pendant de la fonction
        // d'erreur
        var minimumError = GetMinError();

        // Affichage de l'erreur initiale
        Debug.Log(currentError);

        // Initialisation du nombre d'it�rations
    
        // Tout pendant que l'erreur minimale n'est pas atteinte
        while (currentError != GetMinError())
        {
            // On obtient une copie de la solution courante
            // pour ne pas la modifier dans le cas ou la modification
            // ne soit pas conserv�e.
            var newsolution = CopySolution(currentSolution);

            // On proc�de � une petite modification de la solution
            // courante.
            RandomChangeInSolution(newsolution);

            // R�cup�re le score de la nouvelle solution
            // Sachant que l'�valuation peut n�cessiter une 
            // simulation, pour pouvoir la visualiser nous
            // avons recours � une coroutine
            var newscoreEnumerator = GetError(newsolution,time,iterations);
            yield return StartCoroutine(newscoreEnumerator);
            float newError = newscoreEnumerator.Current;

            // On affiche pour des raisons de Debug et de suivi
            // la comparaison entre l'erreur courante et la
            // nouvelle erreur
            Debug.Log(currentError + "   -   " + newError);

            // Si la solution a �t� am�lior�e
            if (newError <= currentError)
            {
                // On met � jour la solution courante
                currentSolution = newsolution;

                // On met � jour l'erreur courante
                currentError = newError;
            }

            // On incr�mente le nombre d'it�rations
            iterations++;

            // On rend la main au moteur Unity3D
            yield return 0;
        }

        // Fin de l'algorithme, on indique que son ex�cution est stopp�e
        _isRunning = false;

        // On affiche le nombre d'it�rations n�cessaire � l'algorithme pour trouver la solution
        Debug.Log("CONGRATULATIONS !!! Solution Found in " + iterations + " iterations !");
    }

    // Coroutine � utiliser pour impl�menter l'algorithme de Djikstra
    public IEnumerator Djikstra()
    {
        var matrix = MatrixFromRaycast.CreateMatrixFromRayCast();


        //TODO
        yield return null;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme d' A*
    public IEnumerator AStar()
    {
        // on cr�e une matrice a 3 dimenstion contenant nos noeud initialis�s
		int[][][] nodes = new int[50][][];
		//On cr�e la matrice et on initialise a -1
		for(int i=0;i < 50;i++){
		nodes[i] = new int[50][];
			for(int j=0;j < 50;j++){
			nodes[i][j]= new int[3];
			nodes[i][j][0]=-1;
			}
		}
		//On r�cupere la position de d�part 
		var player = PlayerScript.CreatePlayer();
		Debug.Log(player.PlayerXPositionInMatrix);
		Debug.Log(player.PlayerYPositionInMatrix);
		//On met l'emplacement de d�part a 0
		nodes[player.PlayerXPositionInMatrix][player.PlayerYPositionInMatrix][0]=0;
		
	
        yield return null;
    }

    // Coroutine � utiliser pour impl�menter l'algorithme du recuit simul�
    public IEnumerator SimulatedAnnealing()
    {
		
		/*
		 * 
		 * Impl�mentation Christophe
		 * Impl�mentation du Recuit Simul�
		 * Lire le README AVANT modification
		 * 
		 */
		

		DateTime time = DateTime.Now;
		DateTime timeEnd = time.AddMinutes(5);
		// Indique que l'algorithme est en cours d'ex�cution
        _isRunning = true;
		
		//G�n�ration de la meilleure erreur obtenue
		var minimumError = GetMinError();
		var nbPath = 42;
		
		// G�n�re une solution initiale au hazard (ici une s�quence
        // de 42 mouvements)
        var currentSolution = new PathSolutionScript(nbPath);
		int iterations = 0;	
		// R�cup�re le score de la solution initiale
        // Sachant que l'�valuation peut n�cessiter une 
        // simulation, pour pouvoir la visualiser nous
        // avons recours � une coroutine
        var scoreEnumerator = GetError(currentSolution,time,iterations);
        yield return StartCoroutine(scoreEnumerator);
        float currentError = scoreEnumerator.Current;
		
		//Initialisation de la temp�rature � une valeur 'plutot basse'.
        float temperature = 2f;
		
		///Initialisation de la valeur de 'stagnation' qui si elle d�passe un
        ///certain seuil provoquera l'augmentation de la temp�rature.
        float stagnation = 0.001f;
		
		// Affichage de l'erreur initiale
        Debug.Log(currentError);
		
		// Initialisation du nombre d'it�rations


        // Tout pendant que l'erreur minimale n'est pas atteinte
        while (currentError != minimumError && DateTime.Compare(DateTime.Now,timeEnd)<0)
        {
		
			// On obtient une copie de la solution courante
            // pour ne pas la modifier dans le cas ou la modification
            // ne soit pas conserv�e.
            var newsolution = CopySolution(currentSolution);

            // On proc�de � une petite modification de la solution
            // courante.
            RandomChangeInSolution(newsolution);
			
			// R�cup�re le score de la nouvelle solution
            // Sachant que l'�valuation peut n�cessiter une 
            // simulation, pour pouvoir la visualiser nous
            // avons recours � une coroutine
            var newscoreEnumerator = GetError(newsolution,time,iterations);
            yield return StartCoroutine(newscoreEnumerator);
            float newError = newscoreEnumerator.Current;
			
			// On affiche pour des raisons de Debug et de suivi
            // la comparaison entre l'erreur courante et la
            // nouvelle erreur
            Debug.Log(currentError + "   -   " + newError);
			Debug.Log("L'erreur min est :" + minimumError);
            Debug.Log("Le nombre d'it�rations est de " + iterations);
			
			
			///Tirage d'un nombre al�atoire entre 0f et 1f.
            float rdm = UnityEngine.Random.Range(0f, 1f);
			
			///Comparaison de ce nombre � la probabilit� d'accepter un changement
            ///d�termin�e par le crit�re de Boltzman.
            if (rdm < BoltzmanCriteria(temperature, currentError, newError))
            {
                ///Si le nombre est inf�rieur, on accepte la solution chang�e
                ///et l'on met � jour l'erreur courante.
                currentError = newError;
				
				//Bien �videment la solution devient la copie de l'ancienne solution
				currentSolution = newsolution;
				
            }
			
			///Si l'erreur stagne
            if (minimumError == currentError)
            {
                ///On incr�mente la stagnation
                stagnation *= 1.001f;
            }
            else
            {
                ///Sinon on la r�initialise
                stagnation = 0.001f;
            }
			
			///Si l'erreur diminue en de�a de la meilleure erreur obtenue
            if (currentError < minimumError)
            {
                ///On met � jour la meilleure erreur obtenue
				///Passage de minimumError en Float
				float minimumErrorF = minimumError;
                minimumErrorF = currentError;

                ///On r�initialise la stagnation
                stagnation = 0.001f;
            }
			
			///On met � jour la temperature � chaque tour de boucle :
            /// - si la stagnation est suffisante la temperature va augmenter
            /// - sinon la temperature d�croit de mani�re g�om�trique
            temperature *= 0.998f + stagnation;

            ///Affichage dans la console de Debug du couple temperature stagnation
            ///pour pouvoir �tre t�moin de l'augmentation de la temp�rature lorsque
            ///l'on se retrouve coinc� trop longtemps dans un minimum local.
            Debug.Log(temperature + "  -  " + stagnation);

            ///On rend la main � Unity pendant un court laps de temps pour permettre
            ///le contr�le de la simulation ainsi que la visualisation de l'�volution
            ///de l'algorithme.
            yield return new WaitForSeconds(0.0001f);
			
			// On incr�mente le nombre d'it�rations
            iterations++;

            // On rend la main au moteur Unity3D
            yield return 0;
			
		}
		
		// Fin de l'algorithme, on indique que son ex�cution est stopp�e
        _isRunning = false;
		
		TimeSpan simulation = DateTime.Now - time;
		string localCsvLine = _csvLog;
		if(DateTime.Compare(DateTime.Now,timeEnd)<0){
        // On affiche le nombre d'it�rations n�cessaire � l'algorithme pour trouver la solution
        Debug.Log("CONGRATULATIONS !!! Solution Found in " + iterations + " iterations !");
		localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iterations+";Optimal;\n";
		Debug.Log("En "+ simulation.TotalSeconds + " secondes !");
		}else{
			Debug.Log("FAIL !!! Solution not Found in " + iterations + " iteration and "+ simulation.TotalSeconds+" Seconds, or "+ simulation.TotalMinutes);
			localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iterations+";Echec;\n";
		}
		File.AppendAllText(@"log.csv",localCsvLine );
			
		
    }
	
	float BoltzmanCriteria(float temperature, float currentError, float newError)
    {
        ///Si la temperature est nulle
        ///cas particulier pour �viter une division par z�ro
        if (temperature == 0)
        {
            return currentError - newError;
        }

        ///Crit�re de Boltzman
        return Mathf.Exp(((float)(currentError - newError)) / temperature);
    }
	
	// Coroutine � utiliser pour impl�menter un algorithme g�n�tique
    public IEnumerator GeneticAlgorithm(int popsize,float mutationRate,float fitness,int mutationSequence)
    {
        
		int iteration = 0;
		DateTime time = DateTime.Now;
		DateTime timeEnd = time.AddMinutes(5);
		#region Param�tres
		
		///La taille de la population


        ///Le nombre d'individus s�l�ctionn�s pour la reproduction
        ///(ici 40% de la taille de la population)
        int numSelection = (int)(popsize * fitness);

        ///Le taux de mutation (c.�.d. la chance avec laquelle un
        ///individu issu du croisement peut subir une mutation)

		
		#endregion
		
		
		/*
		 * 
		 * INITIALISATION
		 * 
		 */ 
		
		
		///Initialisation du tableau contenant notre population initiale
		PathSolutionScript[] population = new PathSolutionScript[popsize];
		
		//G�n�ration de la meilleure erreur obtenue
		var minimumError = GetMinError();
		Debug.Log("Minimum Error to Win : " + minimumError);
		int nbPath = 42;
		var currentSol = new PathSolutionScript(nbPath);
		var sol = new PathSolutionScript(nbPath);

		
		float bestError = 10000;


		
		// R�cup�re le score de la solution initiale
        // Sachant que l'�valuation peut n�cessiter une 
        // simulation, pour pouvoir la visualiser nous
        // avons recours � une coroutine
        var scoreEnumerator = GetError(currentSol,time,iteration);
        yield return StartCoroutine(scoreEnumerator);
        float currentError = scoreEnumerator.Current;
		
		
		
		for (int i = 0; i < popsize; i++)
        {
			//On g�n�re une solution
			population[i] = new PathSolutionScript(nbPath);			
			 
		}
		
		
		
		while (bestError != GetMinError() && DateTime.Compare(DateTime.Now,timeEnd)<0)
		{
			
			///Initialisation du tableau destin� � contenir l'ensemble des
            ///couples chemin/erreur une fois la population �valu�e
            var scoredIndividuals = new ScoredIndividual[popsize];
			
			currentSol = new PathSolutionScript(nbPath);
			
			
            ///Pour chaque individu de notre population � �valuer
			for (int i = 0; i < population.Length; i++)
            {
			
				for (int j = 0; j < population[i].Actions.Length; j++)
              {
					currentSol.Actions[j].Action = population[i].Actions[j].Action;
				}
				
				scoreEnumerator = GetError(population[i],time,iteration);
        		yield return StartCoroutine(scoreEnumerator);
        		currentError = scoreEnumerator.Current;
				
				///Cr�ation d'un couple configuration/solution et stockage
                ///du score obtenu pour la configuration �valu�e.
				scoredIndividuals[i] = new ScoredIndividual()
                {
                    Configuration = population[i],
                    Score = currentError
                };
			}
			
			//operateur de selection
			
			//selection par tournoi 
			PathSolutionScript[] bestIndividuals = new PathSolutionScript[popsize];
			for(int i=0;i< popsize;i++){
			////si l'individu test� est moins bon que l'al�atoire
			int rand = UnityEngine.Random.Range(0,scoredIndividuals.Length);
			if(scoredIndividuals[i].Score >  scoredIndividuals[rand].Score)
			{
				bestIndividuals[i] = scoredIndividuals[rand].Configuration;
			}else{
				bestIndividuals[i] = scoredIndividuals[i].Configuration;
			}
			//	scoredIndividuals[UnityEngine.Random.Range(0,scoredIndividuals.Length)]
			}
			//On initialise notre stockage de best population
			//var bestIndividuals = scoredIndividuals;
			//Methode Roulette Wheel
			//.OrderBy((scoredindi) => scoredindi.Score*UnityEngine.Random.value)        
			//methode Elitiste	
			//.OrderBy((scoredindi) => scoredindi.Score) 
             //.Take(numSelection)
            //.Select((scoredindi) => scoredindi.Configuration)
            //.ToArray();
			
			bestError = scoredIndividuals
                .OrderBy((scoredindi) => scoredindi.Score)
                .Select((scoredindi) => scoredindi.Score).First();
			
			       
            ///Affichage Dans la console de Debug du score du meilleur
            ///individu.
            Debug.Log("Meilleur Score de la g�n�ration courante : " + bestError);
			
	
			///Initialisation de la nouvelle population qui va �tre g�n�r�e
    		///par croisement.
    		PathSolutionScript[] newpopulation = new PathSolutionScript[popsize];
	
	
			///Pour chaque enfant que l'on doit g�n�rer par croisement
    		for (int i = 0; i < popsize; i++)
			{
		
			 	///R�cup�ration de deux reproduteurs au hasard
             	var parent1 = bestIndividuals[UnityEngine.Random.Range(0, bestIndividuals.Length)];
             	var parent2 = bestIndividuals[UnityEngine.Random.Range(0, bestIndividuals.Length)];
	
             	///Cr�ation d'un individu � partir du croisement des deux parents
             	newpopulation[i] = Crossover(parent1, parent2, nbPath);
			}
			
			///Pour chaque individu de la population
        	for (int i = 0; i < popsize; i++)
        	{
	            ///Tirage d'un nombre au hasard entre 0f et 1f
            	float rdm = UnityEngine.Random.Range(0f, 1f);
	            
				///Comparaison de ce nombre au taux de mutation
            	///S'il est inf�rieur, on proc�de � la mutation
            	if (rdm < mutationRate)
				{
	            	///Mutation propos�e :
                	// On proc�de � une petite modification de la solution
                	// courante.
					for(int j=0;j<mutationSequence;j++){
                	RandomChangeInSolution(newpopulation[i+j]);
					}
            	}
        	}
			///Remplacement de l'ancienne population par la nouvelle
        	population = newpopulation;
				
			iteration++;
			
			Debug.Log("Le nombre d'it�rations est de : " + iteration);
	
        	// On rend la main au moteur Unity3D
        	yield return 0;
    	}
	   	// Fin de l'algorithme, on indique que son ex�cution est stopp�e
       	_isRunning = false;
	
       	TimeSpan simulation = DateTime.Now - time;
		string localCsvLine = _csvLog;
		if(DateTime.Compare(DateTime.Now,timeEnd)<0){
	        // On affiche le nombre d'it�rations n�cessaire � l'algorithme pour trouver la solution
	        Debug.Log("CONGRATULATIONS !!! Solution Found in " + iteration + " iterations !");
			localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iteration+";Optimal;\n";
			Debug.Log("En "+ simulation.TotalSeconds + " secondes !");
		}else{
			Debug.Log("FAIL !!! Solution not Found in " + iteration + " iteration and "+ simulation.TotalSeconds+" Seconds, or "+ simulation.TotalMinutes);
			localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iteration+";Echec;\n";
		}
		File.AppendAllText(@"log.csv",localCsvLine );
	    	
	}
		/// <summary>
    /// Structure de donn�e cr��e pour pouvoir stocker les 
    /// associations configuration/score lors de l'�tape
    /// d'�valuation de la population.
    /// </summary>
    class ScoredIndividual
    {
        /// <summary>
        /// La configuration (solution)
        /// </summary>
        public PathSolutionScript Configuration { get; set; }

        /// <summary>
        /// Le score de la configuration ci-dessus
        /// </summary>
        public float Score { get; set; }
    }
	
	    /// <summary>
    /// M�thode proposant une m�thode pour obtenir une nouvel
    /// individu par croisement de deux configurations parentes
    /// </summary>
    /// <param name="parent1">Le parent 1</param>
    /// <param name="parent2">Le parent 2</param>
    /// <returns>L'enfant g�n�r� par croisement</returns>
    PathSolutionScript Crossover(PathSolutionScript parent1, PathSolutionScript parent2, int length)
    {
        PathSolutionScript child = new PathSolutionScript(length);
        for (int i = 0; i < parent1.Actions.Count(); i++)
        {
            if(i%2 != 0)child.Actions[i].Action = parent1.Actions[i].Action;
            else child.Actions[i].Action = parent2.Actions[i].Action;
        }
        return child;
    }

    /// <summary>
    /// Exemple d'erreur minimum (pas forc�ment toujours juste) renvoyant
    /// la distance de manhattan entre la case d'arriv�e et la case de d�part.
    /// </summary>
    /// <returns></returns>
    int GetMinError()
    {
        return (int)(Mathf.Abs(PlayerScript.GoalXPositionInMatrix - PlayerScript.StartXPositionInMatrix) +
            Mathf.Abs(PlayerScript.GoalYPositionInMatrix - PlayerScript.StartYPositionInMatrix));
    }

    /// <summary>
    /// Exemple d'oracle nous renvoyant un score que l'on essaye de minimiser
    /// Ici est utilis� la position de la case d'arriv�e, la position finale
    /// atteinte par la solution. Il est recommand� d'essayer plusieurs oracles
    /// pour �tudier le comportement des algorithmes selon la qualit� de ces
    /// derniers
    /// 
    /// Parmi les param�tres pouvant �tre utilis�s pour calculer le score/erreur :
    /// 
    ///  - position de la case d'arriv�e    : PlayerScript.GoalXPositionInMatrix
    ///                                       PlayerScript.GoalYPositionInMatrix
    ///  - position du joueur               : player.PlayerXPositionInMatrix
    ///                                       player.PlayerYPositionInMatrix
    ///  - position de d�part du joueur     : PlayerScript.StartXPositionInMatrix
    ///                                       PlayerScript.StartYPositionInMatrix
    ///  - nombre de cases explor�es        : player.ExploredPuts
    ///  - nombre d'actions ex�cut�es       : player.PerformedActionsNumber
    ///  - vrai si le la balle a touch� la case d'arriv�e : player.FoundGoal
    ///  - vrai si le la balle a touch� un obstacle : player.FoundObstacle
    ///  - interrogation de la matrice      :
    ///       � la case de coordonn�e (i, j) est elle un obstacle (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == LayerMask.NameToLayer("Obstacle")
    ///       � la case de coordonn�e (i, j) est elle explor�e (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == 1
    ///       � la case de coordonn�e (i, j) est elle inexplor�e (i et j entre 0 et 49) :
    ///           player.GetPutTypeAtCoordinates(i, j) == 0
    /// </summary>
    /// <param name="solution"></param>
    /// <returns></returns>
    IEnumerator<float> GetError(PathSolutionScript solution, System.DateTime time,int iteration)
    {
        // On indique que l'on s'appr�te � lancer la simulation
        _inSimulation = true;

        // On cr�� notre objet que va ex�cuter notre s�quence d'action
        var player = PlayerScript.CreatePlayer();

        // Pour pouvoir visualiser la simulation (moins rapide)
        player.RunWithoutSimulation = false;

        // On lance la simulation en sp�cifiant
        // la s�quence d'action � ex�cuter
        player.LaunchSimulation(solution);

        // Tout pendant que la simulation n'est pas termin�e
        while (player.InSimulation)
        {
            // On rend la main au moteur Unity3D
            yield return -1f;
        }        

        // Calcule la distance de Manhattan entre la case d'arriv�e et la case finale de
        // notre objet, la pond�re (la multiplie par z�ro si le but a �t� trouv�) 
        // et ajoute le nombre d'actions jou�es
        var error = (Mathf.Abs(PlayerScript.GoalXPositionInMatrix - player.PlayerXPositionInMatrix)
            + Mathf.Abs(PlayerScript.GoalYPositionInMatrix - player.PlayerYPositionInMatrix)) 
            * (player.FoundGoal ? 0 : 100)  + 
            player.PerformedActionsNumber;
		
		
		if(player.FoundGoal==true)
		{
			if(!_wasTrue){
				_wasTrue = true;
        	string localCsvLine = _csvLog;
				TimeSpan simulation = DateTime.Now - time;
				localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iteration+";Correct;\n";
				
			Debug.Log("Premier True En "+ simulation.TotalSeconds + " secondes");
			File.AppendAllText(@"log.csv",localCsvLine );
			}
			}
		
        // D�truit  l'objet de la simulation
        Destroy(player.gameObject);

        // Renvoie l'erreur pr�c�demment calcul�e
        yield return error;

        // Indique que la phase de simulation est termin�e
        _inSimulation = false;
    }
	
	
	///<summary>
	/// Oracle pour Astar
	/// Il calcule simplement la distance manhattan 
	/// mais n'ajoute pas 100 si le goal n'est pas trouv�.
	/// Il sera appel� pour chaque case pour determiner sa valeur.
	///</summary>
	 IEnumerator<float> GetAstarError(PathSolutionScript solution, System.DateTime time,int iteration)
    {
        // On indique que l'on s'appr�te � lancer la simulation
        //_inSimulation = true;

        // On cr�� notre objet que va ex�cuter notre s�quence d'action
        var player = PlayerScript.CreatePlayer();

        // Calcule la distance de Manhattan entre la case d'arriv�e et la case finale de
        // notre objet, la pond�re (la multiplie par z�ro si le but a �t� trouv�) 
        // et ajoute le nombre d'actions jou�es
        var error = (Mathf.Abs(PlayerScript.GoalXPositionInMatrix - player.PlayerXPositionInMatrix)
            + Mathf.Abs(PlayerScript.GoalYPositionInMatrix - player.PlayerYPositionInMatrix));
		
		
		if(player.FoundGoal==true)
		{
			if(!_wasTrue){
				_wasTrue = true;
        	string localCsvLine = _csvLog;
				TimeSpan simulation = DateTime.Now - time;
				localCsvLine += "00:00:"+simulation.TotalSeconds+";"+iteration+";Correct;\n";
				
			Debug.Log("Premier True En "+ simulation.TotalSeconds + " secondes");
			File.AppendAllText(@"log.csv",localCsvLine );
			}
			}
		
        // D�truit  l'objet de la simulation
        Destroy(player.gameObject);

        // Renvoie l'erreur pr�c�demment calcul�e
        yield return error;

        // Indique que la phase de simulation est termin�e
        _inSimulation = false;
    }

    /// <summary>
    /// Execute un changement al�atoire sur une solution
    /// ici, une action de la s�quence est tir�e au hasard et remplac�e
    /// par une nouvelle au hasard.
    /// </summary>
    /// <param name="sol"></param>
    public void RandomChangeInSolution(PathSolutionScript sol)
    {
        sol.Actions[UnityEngine.Random.Range(0, sol.Actions.Length)] = new ActionSolutionScript();
    }

    /// <summary>
    /// Fonction utilitaire ayant pour but de copier
    /// dans un nouvel espace m�moire une solution
    /// </summary>
    /// <param name="sol">La solution � copier</param>
    /// <returns>Une copie de la solution</returns>
    public PathSolutionScript CopySolution(PathSolutionScript sol)
    {
        // Initialisation de la nouvelle s�quence d'action
        // de la m�me longueur que celle que l'on souhaite copier
        var newSol = new PathSolutionScript(sol.Actions.Length);

        // Pour chaque action de la s�quence originale,
        // on copie le type d'action.
        for (int i = 0; i < sol.Actions.Length; i++)
        {
            newSol.Actions[i].Action = sol.Actions[i].Action;
        }

        // Renvoi de la solution copi�e
        return newSol;
    }
}


