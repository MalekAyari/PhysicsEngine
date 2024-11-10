using UnityEngine;

public class FreeFallDrag : MonoBehaviour
{
    public Vector3 velocity; // Vitesse du cube
    public float gravity = 9.81f; // Gravit�
    public float dt = 0.002f; // Pas de temps
    public Vector3 position; // Position initiale du cube
    public float mass = 0.5f; // Masse du cube
    public float frictionCoefficient = 100.0f; // Coefficient de frottement (r�sistance de l'air)
    private CustomRBCube customCube; // R�f�rence au script CustomCube

    void Start()
    {
        velocity = Vector3.zero;
        position = Vector3.zero;

        customCube = GetComponent<CustomRBCube>();
    }

    // Utilisation de FixedUpdate pour des calculs physiques
    private void FixedUpdate()
    {
        // Calcul de la force de gravit� : F_gravity = m * g
        Vector3 gravityForce = Vector3.down * gravity * mass;

        // Calcul de la force de frottement : F_friction = -f * |v| * direction(v)
        Vector3 frictionForce = -frictionCoefficient * velocity.magnitude * velocity.normalized;

        // R�sultat de la force totale (F_tot = F_gravity + F_friction)
        Vector3 totalForce = gravityForce + frictionForce;

        // Calcul de l'acc�l�ration : a = F_tot / m
        Vector3 acceleration = totalForce / mass;

        // Appliquer l'acc�l�ration � la vitesse (m�thode d'Euler)
        velocity += acceleration * dt;

        // Mise � jour de la position du cube
        position += velocity * dt;

        // Cr�er une matrice de translation manuellement
        Matrix4x4 translationMatrix = CreateTranslationMatrix(position);

        // Appliquer la matrice de transformation au cube
        customCube.ApplyTransformation(translationMatrix);

        // Condition pour stopper le cube au niveau du sol
        /*
        if (position.y <= 0)
        {
            velocity = Vector3.zero;
            position.y = 0;  // Assurer que le cube ne descende pas en dessous de 0

            // R�-appliquer la matrice de transformation pour stopper le cube
            translationMatrix = CreateTranslationMatrix(position);
            customCube.ApplyTransformation(translationMatrix);
        }
        */
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
