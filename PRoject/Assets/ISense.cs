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
	
	/* Sets the behaviour of the sense. */
	void SetBehaviour (EntityBehaviour behaviour);
}
