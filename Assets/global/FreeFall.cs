using UnityEngine;

public class FreeFall : MonoBehaviour
{
    public Vector3 velocity; // Vitesse du cube
    public float gravity = 9.81f; // Gravit�
    public float dt = 0.002f; // Pas de temps
    public Vector3 position; // Position initiale du cube
    private CustomCube customCube; // R�f�rence au script CustomCube

    void Start()
    {
        // Initialiser la position et la vitesse du cube
        velocity = Vector3.zero;
        position = Vector3.zero; // Le cube commence � l'origine

        // Obtenir la r�f�rence au script CustomCube
        customCube = GetComponent<CustomCube>();
    }

    // Utilisation de FixedUpdate pour des calculs physiques
    private void FixedUpdate()
    {
        // Appliquer la gravit� (chute libre) via m�thode d'Euler
        velocity += Vector3.down * gravity * dt;

        // Mise à jour de la position du cube
        position += velocity * dt;

        // Cr�er une matrice de translation manuellement
        Matrix4x4 translationMatrix = CreateTranslationMatrix(position);

        // Appliquer la matrice de transformation au cube
        customCube.ApplyTransformation(translationMatrix);
    }

    // Fonction pour cr�er une matrice de translation manuellement
    // Cr�ation de la matrice de translation (sans Matrix4x4.Translate)
    Matrix4x4 CreateTranslationMatrix(Vector3 translation)
    {
        Matrix4x4 matrix = Matrix4x4.identity; // Commence par une matrice identit�

        // Remplir les valeurs de translation
        matrix.m03 = translation.x; // D�placement en x
        matrix.m13 = translation.y; // D�placement en y
        matrix.m23 = translation.z; // D�placement en z

        return matrix; // Retourne la matrice de translation
    }
}
