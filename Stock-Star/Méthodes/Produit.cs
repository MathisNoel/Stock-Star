internal class Produit // J'ai du déclarez en internal class après des bug sur l'autre fichier, je ne sais pas pourquoi mais sa marche alors je laisse comme sa
{
    public string Nom { get; set; }
    public decimal PrixAchat { get; set; }
    public decimal? PrixVente { get; set; }
    public decimal Taille { get; set; }
    public string Emplacement { get; set; }
    public string Description { get; set; }

    public decimal? Benefice
    {
        get
        {
            if (PrixVente.HasValue)
                return PrixVente.Value - PrixAchat;

            return null;
        }
    }
}