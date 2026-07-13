import Box from "@mui/material/Box"
import Stack from "@mui/material/Stack"
import { createContext, ReactNode, useContext } from "react"
import { useWizard } from "./Wizard"

const StepContext = createContext<{ stepId: number | string } | null>(null)

export const useStep = () => {
	const ctx = useContext(StepContext)
	if (!ctx) {
		throw new Error("useStep must be used inside <Step>")
	}
	return ctx
}

export const Step = ({
	children,
	stepId = crypto.randomUUID(),
}: {
	children?: ReactNode
	stepId?: number | string
}) => {
	return (
		<StepContext.Provider value={{ stepId }}>
			{children}
		</StepContext.Provider>
	)
}

export const Label = ({ children }: { children: ReactNode }) => {
	return <span>{children}</span>
}

export const StepContent = ({ children }: { children: ReactNode }) => {
	const { currentStep } = useWizard()
	const { stepId } = useStep()
	return (
		<Stack
			className="flex-1 py-5 px-7"
			sx={{
				gridArea: "1 / 1",
				visibility: stepId === currentStep ? "visible" : "hidden",
				pointerEvents: stepId === currentStep ? "auto" : "none",
			}}
		>
			{children}
		</Stack>
	)
}

export const StepHeader = ({ children }: { children: ReactNode }) => {
	const { currentStep, setCurrentStep } = useWizard()
	const { stepId } = useStep()

	return (
		<Stack
			className="cursor-pointer items-center"
			direction="row"
			spacing={2}
			onClick={() => setCurrentStep(stepId)}
		>
			<Box
				className="rounded-full p-2"
				sx={{
					textAlign: "center",
					width: "2.5rem",
					height: "2.5rem",
					backgroundColor:
						stepId === currentStep
							? "primary.main"
							: "secondary.main",
					color:
						stepId === currentStep
							? "primary.contrastText"
							: "secondary.contrastText",
				}}
			>
				{stepId}
			</Box>
			<Label>{children}</Label>
		</Stack>
	)
}
