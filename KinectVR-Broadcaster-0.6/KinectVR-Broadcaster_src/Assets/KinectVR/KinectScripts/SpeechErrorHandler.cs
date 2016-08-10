using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Speech error handler converts SAPI error codes to human readable messages.
/// </summary>
public class SpeechErrorHandler
{

	/// <summary>
	/// Gets the error message for the SAPI error code.
	/// </summary>
	/// <returns>The error message.</returns>
	/// <param name="hr">SAPI error code</param>
	public string GetSapiErrorMessage(int hr)
	{
		uint uhr = (uint)hr;

		if(hresult2message.ContainsKey(uhr))
		{
			return hresult2message[uhr];
		}

		return "0x" + hr.ToString("X");
	}


	private readonly Dictionary<uint, string> hresult2message = new Dictionary<uint, string>
	{
		{0x80045001, "The object has not been properly initialized."},
		{0x80045002, "The object has already been initialized."},
		{0x80045003, "The caller has specified an unsupported format."},
		{0x80045004, "The caller has specified invalid flags for this operation."},
		{0x00045005, "The operation has reached the end of stream."},
		{0x80045006, "The wave device is busy."},
		{0x80045007, "The wave device is not supported."},
		{0x80045008, "The wave device is not enabled."},
		{0x80045009, "There is no wave driver installed."},
		{0x8004500a, "The file must be Unicode."},
		{0x0004500b, "The data is not sufficient."},
		{0x8004500c, "The phrase ID specified does not exist or is out of range."},
		{0x8004500d, "The caller provided a buffer too small to return a result."},
		{0x8004500e, "Caller did not specify a format prior to opening a stream."},
		{0x8004500f, "The stream I/O was stopped by setting the audio object to the stopped state. This will be returned for both read and write streams."},
		{0x00045010, "This will be returned only on input (read) streams when the stream is paused. Reads on paused streams will not block, and this return code indicates that all of the data has been removed from the stream."},
		{0x80045011, "Invalid rule name passed to ActivateGrammar."},
		{0x80045012, "An exception was raised during a call to the current TTS driver."},
		{0x80045013, "An exception was raised during a call to an application sentence filter."},
		{0x80045014, "In speech recognition, the current method cannot be performed while a grammar rule is active."},
		{0x00045015, "The operation was successful, but only with automatic stream format conversion."},
		{0x00045016, "There is currently no hypothesis recognition available."},
		{0x80045017, "Cannot create a new object instance for the specified object category."},
		{0x00045018, "The word, pronunciation, or POS pair being added is already in lexicon."},
		{0x80045019, "The word does not exist in the lexicon."},
		{0x0004501a, "The client is currently synced with the lexicon."},
		{0x8004501b, "The client is excessively out of sync with the lexicon. Mismatches may not sync incrementally."},
		{0x8004501c, "A rule reference in a grammar was made to a named rule that was never defined."},
		{0x8004501d, "A non-dynamic grammar rule that has no body."},
		{0x8004501e, "The grammar compiler failed due to an internal state error."},
		{0x8004501f, "An attempt was made to modify a non-dynamic rule."},
		{0x80045020, "A rule name was duplicated."},
		{0x80045021, "A resource name was duplicated for a given rule."},
		{0x80045022, "Too many grammars have been loaded."},
		{0x80045023, "Circular reference in import rules of grammars."},
		{0x80045024, "A rule reference to an imported grammar that could not be resolved."},
		{0x80045025, "The format of the WAV file is not supported."},
		{0x00045026, "This success code indicates that an SR method called with the SPRIF_ASYNC flag is being processed. When it has finished processing, an SPFEI_ASYNC_COMPLETED event will be generated."},
		{0x80045027, "A grammar rule was defined with a null path through the rule. That is, it is possible to satisfy the rule conditions with no words."},
		{0x80045028, "It is not possible to change the current engine or input. This occurs in the following cases: 1) SelectEngine called while a recognition context exists, or 2) SetInput called in the shared instance case."},
		{0x80045029, "A rule exists with matching IDs (names) but different names (IDs)."},
		{0x8004502a, "A grammar contains no top-level, dynamic, or exported rules. There is no possible way to activate or otherwise use any rule in this grammar."},
		{0x8004502b, "Rule 'A' refers to a second rule 'B' which, in turn, refers to rule 'A'."},
		{0x0004502c, "Parse path cannot be parsed given the currently active rules."},
		{0x8004502d, "Parse path cannot be parsed given the currently active rules."},
		{0x8004502e, "A marshaled remote call failed to respond."},
		{0x8004502f, "This will only be returned on input (read) streams when the stream is paused because the SR driver has not retrieved data recently."},
		{0x80045030, "The result does not contain any audio, nor does the portion of the element chain of the result contain any audio."},
		{0x80045031, "This alternate is no longer a valid alternate to the result it was obtained from. Returned from ISpPhraseAlt methods."},
		{0x80045032, "The result does not contain any audio, nor does the portion of the element chain of the result contain any audio. Returned from ISpResult::GetAudio and ISpResult::SpeakAudio."},
		{0x80045033, "The XML format string for this RULEREF is invalid, e.g. not a GUID or REFCLSID."},
		{0x00045034, "The operation is not supported for stream input."},
		{0x80045035, "The operation is invalid for all but newly created application lexicons."},
		{0x80045036, "SPERR_NO_TERMINATING_RULE_PATH"},
		{0x00045037, "The word exists but without pronunciation."},
		{0x80045038, "An operation was attempted on a stream object that has been closed."},
		{0x80045039, "When enumerating items, the requested index is greater than the count of items."},
		{0x8004503a, "The requested data item (file, data key, value, etc.) was not found."},
		{0x8004503b, "Audio state passed to SetState() is invalid."},
		{0x8004503c, "A generic MMSYS error not caught by _MMRESULT_TO_HRESULT."},
		{0x8004503d, "An exception was raised during a call to the marshaling code."},
		{0x8004503e, "Attempt was made to manipulate a non-dynamic grammar."},
		{0x8004503f, "Cannot add ambiguous property."},
		{0x80045040, "The key specified is invalid."},
		{0x80045041, "The token specified is invalid."},
		{0x80045042, "The xml parser failed due to bad syntax."},
		{0x80045043, "The xml parser failed to load a required resource (e.g., voice, phoneconverter, etc.)."},
		{0x80045044, "Attempted to remove registry data from a token that is already in use elsewhere."},
		{0x80045045, "Attempted to perform an action on an object token that has had associated registry key deleted."},
		{0x80045046, "The selected voice was registered as multi-lingual. SAPI does not support multi-lingual registration."},
		{0x80045047, "Exported rules cannot refer directly or indirectly to a dynamic rule."},
		{0x80045048, "Error parsing the SAPI Text Grammar Format (XML grammar)."},
		{0x80045049, "Incorrect word format, probably due to incorrect pronunciation string."},
		{0x8004504a, "Methods associated with active audio stream cannot be called unless stream is active."},
		{0x8004504b, "Arguments or data supplied by the engine are in an invalid format or are inconsistent."},
		{0x8004504c, "An exception was raised during a call to the current SR engine."},
		{0x8004504d, "Stream position information supplied from engine is inconsistent."},
		{0x0004504e, "Operation could not be completed because the recognizer is inactive. It is inactive either because the recognition state is currently inactive or because no rules are active."},
		{0x8004504f, "When making a remote call to the server, the call was made on the wrong thread."},
		{0x80045050, "The remote process terminated unexpectedly."},
		{0x80045051, "The remote process is already running; it cannot be started a second time."},
		{0x80045052, "An attempt to load a CFG grammar with a LANGID different than other loaded grammars."},
		{0x00045053, "A grammar-ending parse has been found that does not use all available words."},
		{0x80045054, "An attempt to deactivate or activate a non top-level rule."},
		{0x00045055, "An attempt to parse when no rule was active."},
		{0x80045056, "An attempt to ask a container lexicon for all words at once."},
		{0x00045057, "An attempt to activate a rule/dictation/etc without calling SetInput first in the InProc case."},
		{0x80045059, "The requested language is not supported."},
		{0x8004505a, "The operation cannot be performed because the voice is currently paused."},
		{0x8004505b, "This will only be returned on input (read) streams when the real time audio device stops returning data for a long period of time."},
		{0x8004505c, "An audio device stopped returning data from the Read() method even though it was in the run state. This error is only returned in the END_SR_STREAM event."},
		{0x8004505d, "The SR engine is unable to add this word to a grammar. The application may need to supply an explicit pronunciation for this word."},
		{0x8004505e, "An attempt to call ScaleAudio on a recognition result having previously called GetAlternates. Allowing the call to succeed would result in the previously created alternates located in incorrect audio stream positions."},
		{0x8004505f, "The method called is not supported for the shared recognizer. For example, ISpRecognizer::GetInputStream()."},
		{0x80045060, "A task could not complete because the SR engine had timed out."},
		{0x80045061, "An SR engine called synchronize while inside of a synchronize call."},
		{0x80045062, "The grammar contains a node no arcs."},
		{0x80045063, "Neither audio output nor input is supported for non-active console sessions."},
		{0x80045064, "The object is a stale reference and is invalid to use. For example, having an ISpeechGrammarRule object reference and then calling ISpeechRecoGrammar::Reset() will cause the rule object to be invalidated. Calling any methods after this will result in this error."},
		{0x00045065, "This can be returned from Read or Write calls for audio streams when the stream is stopped."},
		{0x80045066, "The Recognition Parse Tree could not be generated. For example, a rule name begins with a digit but the XML parser does not allow an element name beginning with a digit."},
		{0x80045067, "The SML could not be generated. For example, the transformation xslt template is not well formed."},
		{0x80045068, "The SML could not be generated. For example, the transformation xslt template is not well formed."},
		{0x80045069, "There is already a root rule for this grammar. Defining another root rule will fail."},
		{0x80045070, "Support for embedded script not supported because browser security settings have disabled it."},
		{0x80045071, "A time out occurred starting the SAPI server."},
		{0x80045072, "A timeout occurred obtaining the lock for starting or connecting to SAPI server."},
		{0x80045073, "When there is a cfg grammar loaded, changing the security manager is not permitted."},
		{0x00045074, "Parse is valid but could be extendable (internal use only)."},
		{0x80045075, "Tried and failed to delete an existing file."},
		{0x80045076, "The user has chosen to disable speech from running on the machine, or the system is not set up to run speech (for example, initial setup and tutorial has not been run)."},
		{0x80045077, "No recognizer is installed."},
		{0x80045078, "No audio device is installed."},
		{0x80045079, "No vowel in a word."},
		{0x8004507A, "No vowel in a word."},
		{0x0004507B, "The grammar does not have any root or top-level active rules to activate."},
		{0x0004507C, "The engine does not need SAPI word entry handles for this grammar."},
		{0x8004507D, "The word passed to the GetPronunciations interface needs normalizing first."},
		{0x8004507E, "The word passed to the normalize interface cannot be normalized."},
		{0x80045080, "This combination of function call and input is currently not supported."},

		{0x80070015, "Device not ready."},
	};
}
