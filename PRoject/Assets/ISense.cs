/* Basic methods for getting feedback from senses of etities. */
public interface ISense
{
	/* Do we have a target we are tracking. */
	bool HasTarget ();
	
	/* Attempts to acquire and returns a tracked target. In any frame if we have a target
	   we must return a valid GameObject for it. */
	UnityEngine.GameObject GetTarget ();
	
	/* Removes all traces of a target. */
	void DropTarget ();
	
	/* Returns the last known location of the last target tracked. Independent of whether
	   we have a target or not. Can return null for edge cases. */
	UnityEngine.Vector3 GetLastPosition ();
	
	/* Sets the behaviour of the sense. */
	void SetBehaviour (EntityBehaviour behaviour);
}
