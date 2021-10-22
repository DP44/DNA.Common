using System;

namespace DNA.Security.Cryptography.Asn1.X509
{
	public class AlgorithmIdentifier : Asn1Encodable
	{
		private readonly DerObjectIdentifier objectID;

		private readonly Asn1Encodable parameters;

		public static AlgorithmIdentifier GetInstance(Asn1TaggedObject obj, bool explicitly)
		{
			return AlgorithmIdentifier.GetInstance(Asn1Sequence.GetInstance(obj, explicitly));
		}

		public static AlgorithmIdentifier GetInstance(object obj)
		{
			if (obj == null || obj is AlgorithmIdentifier)
			{
				return (AlgorithmIdentifier)obj;
			}
			if (obj is DerObjectIdentifier)
			{
				return new AlgorithmIdentifier((DerObjectIdentifier)obj);
			}
			if (obj is string)
			{
				return new AlgorithmIdentifier((string)obj);
			}
			if (obj is Asn1Sequence)
			{
				return new AlgorithmIdentifier((Asn1Sequence)obj);
			}
			throw new ArgumentException("unknown object in factory: " + obj.GetType().Name, "obj");
		}

		public AlgorithmIdentifier(DerObjectIdentifier objectID)
		{
			this.objectID = objectID;
		}

		public AlgorithmIdentifier(string objectID)
		{
			this.objectID = new DerObjectIdentifier(objectID);
		}

		public AlgorithmIdentifier(DerObjectIdentifier objectID, Asn1Encodable parameters)
		{
			this.objectID = objectID;
			this.parameters = parameters;
		}

		internal AlgorithmIdentifier(Asn1Sequence seq)
		{
			if (seq.Count < 1 || seq.Count > 2)
			{
				throw new ArgumentException("Bad sequence size: " + seq.Count);
			}
			this.objectID = DerObjectIdentifier.GetInstance(seq[0]);
			if (seq.Count == 2)
			{
				this.parameters = seq[1];
			}
		}

		public virtual DerObjectIdentifier ObjectID
		{
			get
			{
				return this.objectID;
			}
		}

		public Asn1Encodable Parameters
		{
			get
			{
				return this.parameters;
			}
		}

		public override Asn1Object ToAsn1Object()
		{
			Asn1EncodableVector asn1EncodableVector = new Asn1EncodableVector(new Asn1Encodable[]
			{
				this.objectID
			});
			if (this.parameters != null)
			{
				asn1EncodableVector.Add(new Asn1Encodable[]
				{
					this.parameters
				});
			}
			return new DerSequence(asn1EncodableVector);
		}
	}
}
