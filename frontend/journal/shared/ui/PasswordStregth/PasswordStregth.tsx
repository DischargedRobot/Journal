import { memo, useCallback, useMemo } from "react"

interface Props {
	password: string
}
const messages = ["Никакущий", "Слабый", "Средний", "Сильный", "Очень сильный"]
const calculateStrength = (password: string) => {
	let strength = 0

	if (/\d/.test(password)) {
		strength += 1
	}
	if (/\W/.test(password)) {
		strength += 1
	}
	if (password.length >= 8) {
		strength += Math.trunc(password.length / 8)
	} else {
		strength = 0
	}

	return Math.min(strength, messages.length - 1)
}

const selectColorStrength = (strength: number) => {
	switch (strength) {
		case 0:
			return "bg-gray-300"
		case 1:
			return "bg-red-500"
		case 2:
			return "bg-yellow-500"
		case 3:
			return "bg-green-400"
		case 4:
			return "bg-green-600"
		default:
			return "bg-gray-300"
	}
}

const PasswordStregth = ({ password }: Props) => {
	const strength = useMemo(() => calculateStrength(password), [password])
	const colorStrength = useMemo(
		() => selectColorStrength(strength),
		[strength],
	)

	return (
		<span className="block w-full">
			<span className="flex items-center gap-4 w-full ">
				<span className="flex gap-2 w-full">
					<span className={`max-w-20 w-full h-1 rounded-full bg-gray-300 ${colorStrength}`} />
					<span className={`max-w-20 w-full h-1 rounded-full bg-gray-300 ${strength > 1 ? colorStrength : ""}`} />
					<span className={`max-w-20 w-full h-1 rounded-full bg-gray-300 ${strength > 2 ? colorStrength : ""}`} />
					<span className={`max-w-20 w-full h-1 rounded-full bg-gray-300 ${strength > 3 ? colorStrength : ""}`} />
				</span>
				<span className="w-full">{messages[strength]}</span>
			</span>
		</span>
	)
}

export default memo(PasswordStregth)
